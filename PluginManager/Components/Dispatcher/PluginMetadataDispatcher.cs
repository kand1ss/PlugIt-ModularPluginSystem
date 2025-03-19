using System.Reflection;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public class PluginMetadataDispatcher(IAssemblyMetadataRepository repository, IPluginMetadataService metadataService, 
    IAssemblyLoader loader, IAssemblyHandler handler, IPluginLifecycleManager lifecycleManager, 
    PluginLoggerLayer logger)
{
    private readonly AssemblyMetadataGenerator _metadataGenerator = new(handler);
    
    private IEnumerable<string> GetAssembliesNames(IEnumerable<Assembly> assemblies)
        => assemblies.Select(assembly => assembly.GetName().Name ?? string.Empty);
    
    public void RebuildMetadata()
    {
        repository.Clear();
        lifecycleManager.Clear();
        
        var assemblies = loader.LoadAllAssemblies();
        var assemblyNames = GetAssembliesNames(assemblies);
        
        foreach (var assemblyName in assemblyNames)
            LoadMetadata(assemblyName);
    }

    public void RemoveMetadata(string assemblyName)
    {
        var metadata = metadataService.GetMetadata(assemblyName);
        var plugins = metadataService.GetPluginNamesFromMetadata(metadata);
        
        repository.Remove(assemblyName);
        lifecycleManager.RemovePlugins(plugins);
        logger.MetadataRemoved(assemblyName, metadata.Version);
    }
    
    public void LoadMetadata(string assemblyName)
    {
        var assembly = loader.LoadAssembly(assemblyName);
        var metadata = _metadataGenerator.Generate(assembly);

        MetadataValidator.Validate(metadata);
        repository.Add(metadata);
        logger.MetadataAdded(assemblyName, metadata.Version);

        var plugins = metadataService.GetPluginNamesFromMetadata(metadata);
        lifecycleManager.SetPluginsState(plugins, PluginState.Unloaded);
    }
    
    public void UpdateMetadata(string assemblyName)
    {
        RemoveMetadata(assemblyName);
        LoadMetadata(assemblyName);
    }
}
