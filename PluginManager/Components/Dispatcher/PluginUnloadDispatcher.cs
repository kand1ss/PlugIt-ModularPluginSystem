using System.Collections;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class PluginUnloadDispatcher(IPluginMetadataService metadataService, IAssemblyLoader loader, 
    IPluginLifecycleManager lifecycleManager, PluginLoggingFacade logger)
{
    private IEnumerable<string> GetPluginNamesFromAssembly(string assemblyName)
    {
        var metadata = metadataService.GetMetadata(assemblyName);
        return metadataService.GetPluginNamesFromMetadata(metadata);
    }
    
    public void UnloadAssembly(string assemblyName)
    {
        var pluginNames = GetPluginNamesFromAssembly(assemblyName);
        loader.UnloadAssembly(assemblyName);
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Unloaded);
    }
    
    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        var plugin = metadataService.GetPluginMetadataFromAssembly(metadata, pluginName);
        foreach (var dependency in plugin.Dependencies)
            UnloadAssemblyByPluginName(dependency.Name);
        
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
