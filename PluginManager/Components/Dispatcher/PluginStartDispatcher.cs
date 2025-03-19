using ModularPluginAPI.Components.Interfaces.Services;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginStartDispatcher(IPluginMetadataService metadataService, IPluginLoaderService loaderService, 
    IAssemblyLoader loader, IPluginExecutor pluginExecutor, IPluginDependencyResolver dependencyResolver)
{
    
    public void StartPlugin(string pluginName)
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var plugin = loaderService.TryGetPlugin<IPlugin>(assembly, pluginName);
        dependencyResolver.Resolve(plugin);
        
        pluginExecutor.Execute(plugin);
    }

    public void StartPlugins(IEnumerable<string> pluginNames)
    {
        foreach (var pluginName in pluginNames)
            StartPlugin(pluginName);
    }
    
    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        var metadata = metadataService.GetMetadata(assemblyName);
        MetadataValidator.Validate(metadata);
        
        var plugins = metadataService.GetPluginNamesFromMetadata(metadata);
        StartPlugins(plugins);
    }
    
    public void StartAllPlugins()
    {
        foreach (var assembly in loader.GetAllAssembliesNames())
            StartAllPluginsFromAssembly(assembly);
    }
    
    public void StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var extensionPlugin = loaderService.TryGetPlugin<IExtensionPlugin<T>>(assembly, pluginName);
        dependencyResolver.Resolve(extensionPlugin);
        
        pluginExecutor.ExecuteExtensionPlugin(ref data, extensionPlugin);
    }
    
    
    public byte[] ReceiveNetworkPlugin(string pluginName)
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var networkPlugin = loaderService.TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        dependencyResolver.Resolve(networkPlugin);
        
        return pluginExecutor.ExecuteNetworkPluginReceive(networkPlugin);
    }
    
    public void SendNetworkPlugin(string pluginName, byte[] data)
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var networkPlugin = loaderService.TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        dependencyResolver.Resolve(networkPlugin);
        
        pluginExecutor.ExecuteNetworkPluginSend(data, networkPlugin);
    }
}