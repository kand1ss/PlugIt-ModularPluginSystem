using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class PluginUnloadDispatcher(IAssemblyLoader loader, IAssemblyMetadataRepository repository,
    IPluginLifecycleManager lifecycleManager)
{
    private AssemblyMetadata TryGetMetadata(string assemblyName)
    {
        var metadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (metadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
        MetadataValidator.Validate(metadata);
        return metadata;
    }

    private AssemblyMetadata TryGetMetadataByPluginName(string pluginName)
    {
        var metadata = repository.GetMetadataByPluginName(pluginName);
        if (metadata is null)
            throw new PluginNotFoundException(pluginName);
        
        MetadataValidator.Validate(metadata);
        return metadata;
    }
    
    
    public void UnloadAssembly(string assemblyName)
    {
        var metadata = TryGetMetadata(assemblyName);
        var pluginNames = metadata.Plugins.Select(p => p.Name);
        
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Unloaded);
        loader.UnloadAssembly(assemblyName);
    }
    
    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var metadata = TryGetMetadataByPluginName(pluginName);
        var plugin = metadata.Plugins.FirstOrDefault(p => p.Name == pluginName);
        
        var pluginDependencies = plugin?.Dependencies ?? [];
        var dependenciesMetadata = pluginDependencies
            .Keys.Select(TryGetMetadataByPluginName);
        
        UnloadAssemblies(dependenciesMetadata.Select(p => p.Name));
        UnloadAssembly(metadata.Name);
    }
    
    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
    {
        foreach (var assemblyName in assemblyNames)
            UnloadAssembly(assemblyName);
    }
    
    public void UnloadAllAssemblies()
    {
        var assemblyPaths = Directory.GetFiles(loader.GetPluginPath(), "*.dll");
        var assemblyNames = assemblyPaths.Select(Path.GetFileNameWithoutExtension).OfType<string>();
        
        UnloadAssemblies(assemblyNames);
    }
}
