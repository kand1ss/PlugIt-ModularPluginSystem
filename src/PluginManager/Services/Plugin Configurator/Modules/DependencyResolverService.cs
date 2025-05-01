using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;
using PluginAPI.Dependency;

namespace ModularPluginAPI.Components;

public class DependencyResolverService(IPluginLoaderService loader, IPluginMetadataService metadataService,
    PluginLoggingFacade logger) : IDependencyResolverService
{
    private readonly HashSet<string> _loadingPlugins = new();

    private void TryGetDependency(string pluginName, List<IPluginData> loadedDependencies)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        var assembly = loader.LoadAssemblyByPluginName(pluginName);
        var plugin = loader.TryGetPlugin<IPluginData>(assembly, pluginName);
        
        loadedDependencies.Add(plugin);
        logger.DependencyLoaded(plugin.Name, plugin.Version, metadata.Name, metadata.Version);
    }
    
    private static PluginMetadata? GetPlugin(IEnumerable<PluginMetadata> allPlugins, string pluginName)
        => allPlugins.FirstOrDefault(p => p.Name == pluginName);

    private static bool CheckDependencyIsValid(PluginMetadata? pluginFromList, Version pluginVersion)
        => pluginFromList is not null && pluginFromList.Version >= pluginVersion;

    private List<IPluginData> FindDependencies(IEnumerable<DependencyInfo> requiredDependencies)
    {
        var allPlugins = metadataService.GetAllPluginsMetadata();
        var loadedDependencies = new List<IPluginData>();
        
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

    private void CheckCyclicDependency(IPluginData plugin)
    {
        if (!_loadingPlugins.Add(plugin.Name))
        {
            var dependencyPath = string.Join(" -> ", _loadingPlugins) + " -> " + plugin.Name;
            _loadingPlugins.Clear();
            throw new ResolvingDependencyException($"Cyclic dependency detected: [{dependencyPath}]");
        }
    }

    public void Resolve(IPluginData plugin)
        => ResolveWithResult(plugin);

    public IEnumerable<IPluginData> ResolveWithResult(IPluginData plugin)
    {
        if (plugin is not IPluginWithDependencies configurable || configurable.Configuration is null)
            return [];
        
        CheckCyclicDependency(plugin);
        var dependencies = configurable.Configuration.Dependencies;
        var loadedDependencies = FindDependencies(dependencies);
        var totalLoadedDependencies = new List<IPluginData>(loadedDependencies);
        
        foreach (var dependency in loadedDependencies)
        {
            totalLoadedDependencies.AddRange(ResolveWithResult(dependency));
            configurable.LoadDependency(dependency);
        }
        
        _loadingPlugins.Remove(plugin.Name);
        return totalLoadedDependencies;
    }
}