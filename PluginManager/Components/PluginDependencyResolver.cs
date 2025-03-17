using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

// TODO - добавить обнаружение циклических зависимостей
public class PluginDependencyResolver(IAssemblyMetadataRepository repository, IAssemblyLoader assemblyLoader,
    IAssemblyHandler handler, ILoggerService logger) : IPluginDependencyResolver
{
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

    public void LoadPlugin(IPlugin plugin)
    {
        if (plugin is not IPluginWithDependencies configurable || configurable.Configuration is null)
            return;

        var dependencies = configurable.Configuration.Dependencies;
        var loadedDependencies = FindDependencies(dependencies);

        foreach (var dependency in loadedDependencies)
            configurable.LoadDependency(dependency);
    }
}