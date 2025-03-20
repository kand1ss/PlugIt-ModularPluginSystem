using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;
using PluginAPI.Dependency;

namespace ModularPluginAPI.Components;

public class PluginDependencyResolver(IAssemblyMetadataRepository repository, IAssemblyLoader assemblyLoader,
    IAssemblyHandler handler, PluginLoggingFacade logger) : IPluginDependencyResolver
{
    private readonly HashSet<string> _loadingPlugins = new();

    private void TryGetDependency(string pluginName, List<IPlugin> loadedDependencies)
    {
        var assemblyMetadata = repository.GetMetadataByPluginName(pluginName)
            ?? throw new ResolvingDependencyException(pluginName);
        
        var assembly = assemblyLoader.LoadAssembly(assemblyMetadata.Name)
            ?? throw new AssemblyNotFoundException(assemblyMetadata.Name);
                    
        var loadedPlugin = handler.GetPlugin<IPlugin>(assembly, pluginName)
            ?? throw new PluginNotFoundException(pluginName, assemblyMetadata.Name);
        
        loadedDependencies.Add(loadedPlugin);
        logger.DependencyLoaded(loadedPlugin.Name, loadedPlugin.Version, assemblyMetadata.Name, 
            assemblyMetadata.Version);
    }

    private HashSet<PluginMetadata> GetAllPlugins()
        => repository.GetAllMetadata()
            .SelectMany(m => m.Plugins)
            .ToHashSet();
    
    private static PluginMetadata? GetPlugin(IEnumerable<PluginMetadata> allPlugins, string pluginName)
        => allPlugins.FirstOrDefault(p => p.Name == pluginName);

    private static bool CheckDependencyIsValid(PluginMetadata? pluginFromList, Version pluginVersion)
        => pluginFromList is not null && pluginFromList.Version >= pluginVersion;

    private List<IPlugin> FindDependencies(IEnumerable<DependencyInfo> requiredDependencies)
    {
        var allPlugins = GetAllPlugins();
        var loadedDependencies = new List<IPlugin>();
        
        foreach (var dependency in requiredDependencies)
        {
            var pluginName = dependency.Name;
            var pluginVersion = dependency.Version;
            var pluginFromList = GetPlugin(allPlugins, pluginName);
            
            if (CheckDependencyIsValid(pluginFromList, pluginVersion))
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