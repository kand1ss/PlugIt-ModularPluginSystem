using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class PluginMetadataDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader,
    IPluginLifecycleManager lifecycleManager, AssemblyMetadataGenerator metadataGenerator,
    ILoggerService logger)
{
    private AssemblyMetadata TryGetMetadata(string assemblyName)
    {
        var metadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (metadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
        return metadata;
    }
    
    public void RebuildMetadata()
    {
        repository.Clear();
        lifecycleManager.Clear();
        
        var assemblies = loader.LoadAllAssemblies();
        var assemblyNames = assemblies
            .Select(assembly => assembly.GetName().Name ?? string.Empty);
        
        foreach (var assemblyName in assemblyNames)
            LoadMetadata(assemblyName);
    }

    public void RemoveMetadata(string assemblyName)
    {
        var metadata = TryGetMetadata(assemblyName);
        var plugins = metadata.Plugins.Select(x => x.Name);
        
        repository.Remove(assemblyName);
        lifecycleManager.RemovePlugins(plugins);
        
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Metadata for assembly '{assemblyName} v{metadata.Version}' removed.");
    }
    
    public void LoadMetadata(string assemblyName)
    {
        var assembly = loader.LoadAssembly(assemblyName);
        var metadata = metadataGenerator.Generate(assembly);
        var plugins = metadata.Plugins.Select(x => x.Name);
        
        MetadataValidator.Validate(metadata);
        
        repository.Add(metadata);
        lifecycleManager.SetPluginsState(plugins, PluginState.Unloaded);
        
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Metadata for assembly '{assemblyName} v{metadata.Version}' added.");
    }
    
    public void UpdateMetadata(string assemblyName)
    {
        var oldMetadata = TryGetMetadata(assemblyName);
        RemoveMetadata(assemblyName);
        LoadMetadata(assemblyName);
        
        var newMetadata = TryGetMetadata(assemblyName);
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Metadata for assembly '{assemblyName} v{oldMetadata.Version}' updated to '{assemblyName} v{newMetadata.Version}'.");
    }
}
