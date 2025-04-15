using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components;

public class PluginUnloadDispatcher(IPluginMetadataService metadataService, IAssemblyLoader loader, 
    IPluginTracker tracker)
{
    public void UnloadAssembly(string assemblyPath)
    {
        var metadata = metadataService.GetMetadata(assemblyPath);
        var pluginNames = metadataService.GetPluginNamesFromMetadata(metadata);
        
        loader.UnloadAssembly(assemblyPath);
        tracker.SetPluginsState(pluginNames, PluginState.Unloaded);
    }
    
    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        var plugin = metadataService.GetPluginMetadataFromAssembly(metadata, pluginName);
        
        if (plugin.HasConfiguration)
            foreach (var dependency in plugin.Configuration.Dependencies)
                UnloadAssemblyByPluginName(dependency.Name);
        
        UnloadAssembly(metadata.Path);
    }
    
    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
    {
        foreach (var assemblyName in assemblyNames)
            UnloadAssembly(assemblyName);
    }

    public void UnloadAssembliesFromDirectory(string directoryPath)
    {
        var assemblies = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach(var assembly in assemblies)
            UnloadAssembly(assembly);
    }
    
    public void UnloadAllAssemblies()
    {
        var assemblyPaths = metadataService.GetAllAssembliesPaths();
        UnloadAssemblies(assemblyPaths);
    }
}
