using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Plugin_Configurator.Interfaces;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginStartDispatcher(IPluginMetadataService metadataService, IPluginLoaderService loaderService, 
    IPluginExecutor pluginExecutor, IPluginConfiguratorService configuratorService, PluginLoggingFacade logger)
{
    private T GetPluginFromAssembly<T>(string pluginName) where T : class, IPlugin
    {
        var assembly = loaderService.LoadAssemblyByPluginName(pluginName);
        var plugin = loaderService.TryGetPlugin<T>(assembly, pluginName);
        
        var assemblyName = assembly.GetName().Name ?? "null";
        var assemblyVersion = assembly.GetName().Version ?? new Version(0, 0, 0);
        
        logger.PluginLoaded(plugin.Name, assemblyName, assemblyVersion);
        logger.PluginStateChanged(pluginName, PluginState.Loaded);
        
        configuratorService.Configure(plugin);
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
    
    public void StartAllPluginsFromAssembly(string assemblyPath)
    {
        var metadata = metadataService.GetMetadata(assemblyPath);
        MetadataValidator.Validate(metadata);
        
        var plugins = metadataService.GetPluginNamesFromMetadata(metadata);
        StartPlugins(plugins);
    }
    
    public void StartAllPlugins()
    {
        foreach (var assembly in metadataService.GetAllAssembliesPaths())
            StartAllPluginsFromAssembly(assembly);
    }
    
    public void StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        var plugin = GetPluginFromAssembly<IExtensionPlugin<T>>(pluginName);
        pluginExecutor.ExecuteExtensionPlugin(ref data, plugin);
    }
    
    
    public async Task<byte[]> ReceiveNetworkPluginAsync(string pluginName)
    {
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        return await pluginExecutor.ExecuteNetworkPluginReceiveAsync(plugin);
    }
    
    public async Task SendNetworkPluginAsync(string pluginName, byte[] data)
    {
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        await pluginExecutor.ExecuteNetworkPluginSendAsync(data, plugin);
    }

    public async Task<byte[]> ReadFilePluginAsync(string pluginName)
    {
        var plugin = GetPluginFromAssembly<IFilePlugin>(pluginName);
        return await pluginExecutor.ExecuteFilePluginReadAsync(plugin);
    }

    public async Task WriteFilePluginAsync(string pluginName, byte[] data)
    {
        var plugin = GetPluginFromAssembly<IFilePlugin>(pluginName);
        await pluginExecutor.ExecuteFilePluginWriteAsync(data, plugin);
    }
}