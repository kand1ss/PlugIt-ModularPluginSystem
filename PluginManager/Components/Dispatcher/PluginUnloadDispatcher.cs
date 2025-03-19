using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components;

public class PluginUnloadDispatcher(IPluginMetadataService metadataService, IAssemblyLoader loader, 
    IPluginLifecycleManager lifecycleManager)
{
    public void UnloadAssembly(string assemblyName)
    {
        var metadata = metadataService.GetMetadata(assemblyName);
        var pluginNames = metadataService.GetPluginNamesFromMetadata(metadata);

        loader.UnloadAssembly(assemblyName);
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Unloaded);
    }
    
    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        var plugin = metadataService.GetPluginMetadataFromAssembly(metadata, pluginName);
        
        foreach (var dependency in plugin.Dependencies.Keys)
            UnloadAssemblyByPluginName(dependency);
        
        UnloadAssembly(metadata.Name);
    }
    
    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
    {
        foreach (var assemblyName in assemblyNames)
            UnloadAssembly(assemblyName);
    }
    
    public void UnloadAllAssemblies()
    {
        var assemblyNames = loader.GetAllAssembliesNames();
        UnloadAssemblies(assemblyNames);
    }
}
