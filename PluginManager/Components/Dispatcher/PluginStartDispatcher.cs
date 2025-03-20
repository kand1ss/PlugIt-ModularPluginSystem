using ModularPluginAPI.Components.Interfaces.Services;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginStartDispatcher(IPluginMetadataService metadataService, IPluginLoaderService loaderService, 
    IAssemblyLoader loader, IPluginExecutor pluginExecutor, IPluginDependencyResolver dependencyResolver)
{
    private T GetPluginFromAssembly<T>(string pluginName) where T : class, IPlugin
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var plugin = loaderService.TryGetPlugin<T>(assembly, pluginName);
        dependencyResolver.Resolve(plugin);
        
        return plugin;
    }

    public void StartPlugin(string pluginName)
    {
        var plugin = GetPluginFromAssembly<IPlugin>(pluginName);
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
        var plugin = GetPluginFromAssembly<IExtensionPlugin<T>>(pluginName);
        pluginExecutor.ExecuteExtensionPlugin(ref data, plugin);
    }
    
    
    public byte[] ReceiveNetworkPlugin(string pluginName)
    {
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        return pluginExecutor.ExecuteNetworkPluginReceive(plugin);
    }
    
    public void SendNetworkPlugin(string pluginName, byte[] data)
    {
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        pluginExecutor.ExecuteNetworkPluginSend(data, plugin);
    }
}