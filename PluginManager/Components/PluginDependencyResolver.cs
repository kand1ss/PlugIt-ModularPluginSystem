using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginDependencyResolver(IAssemblyMetadataRepository repository, IAssemblyLoader assemblyLoader,
    IAssemblyHandler handler, ILoggerService logger) : IPluginDependencyResolver
{
    private readonly HashSet<string> _loadingPlugins = new();

    private void TryGetDependency(string pluginName, List<IPlugin> loadedDependencies)
    {
        var assemblyMetadata = repository.GetMetadataByPluginName(pluginName);
        if (assemblyMetadata is null)
            throw new ResolvingDependencyException(pluginName);
        
        var assembly = assemblyLoader.LoadAssembly(assemblyMetadata.Name);
        if (assembly is null)
            throw new AssemblyNotFoundException(assemblyMetadata.Name);
                    
        var loadedPlugin = handler.GetPlugin<IPlugin>(assembly, pluginName);
        if (loadedPlugin is null)
            throw new PluginNotFoundException(pluginName, assemblyMetadata.Name);
        
        loadedDependencies.Add(loadedPlugin);
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Dependency '{loadedPlugin.Name} v{loadedPlugin.Version}' from assembly '{assemblyMetadata.Name} v{assemblyMetadata.Version}' loaded.");
    }

    private HashSet<PluginMetadata> GetAllPlugins()
        => repository.GetAllMetadata()
            .SelectMany(m => m.Plugins)
            .ToHashSet();

    private List<IPlugin> FindDependencies(Dictionary<string, Version> requiredDependencies)
    {
        var allPlugins = GetAllPlugins();
        var loadedDependencies = new List<IPlugin>();
        
        foreach (var (pluginName, pluginVersion) in requiredDependencies)
        {
            var pluginFromList = allPlugins.FirstOrDefault(p => p.Name == pluginName);

            if (pluginFromList is not null && pluginFromList.Version >= pluginVersion)
                TryGetDependency(pluginName, loadedDependencies);
            else
                throw new ResolvingDependencyException(pluginName);
        }
        return loadedDependencies;
    }

    private void CheckCyclicDependency(IPlugin plugin)
    {
        if (!_loadingPlugins.Add(plugin.Name))
        {
            var dependencyPath = string.Join(" -> ", _loadingPlugins) + " -> " + plugin.Name;
            _loadingPlugins.Clear();
            throw new ResolvingDependencyException($"Cyclic dependency detected: [{dependencyPath}]");
        }
    }

    public void Resolve(IPlugin plugin)
    {
        if (plugin is not IPluginWithDependencies configurable || configurable.Configuration is null)
            return;
        
        CheckCyclicDependency(plugin);
        var dependencies = configurable.Configuration.Dependencies;
        var loadedDependencies = FindDependencies(dependencies);
        
        foreach (var dependency in loadedDependencies)
        {
            Resolve(dependency);
            configurable.LoadDependency(dependency);
        }
        
        _loadingPlugins.Remove(plugin.Name);
    }
}