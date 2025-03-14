using System.Reflection;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginStartDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader,
    IAssemblyHandler handler, IPluginExecutor pluginExecutor, ILoggerService logger)
{
    private AssemblyMetadata TryGetMetadataByPluginName(string pluginName)
    {
        var metadata = repository.GetMetadataByPluginName(pluginName);
        if (metadata is null)
            throw new PluginNotFoundException(pluginName);
        
        MetadataValidator.Validate(metadata);
        return metadata;
    }

    private Assembly LoadAssemblyByPluginName(string pluginName)
    {
        var metadata = TryGetMetadataByPluginName(pluginName);
        return loader.LoadAssembly(metadata.Name);
    }

    private T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
    {
        var plugin = handler.GetPlugin<T>(assembly, pluginName);
        if (plugin is null)
            throw new PluginNotFoundException(pluginName);
        
        var assemblyName = assembly.GetName();
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Plugin '{pluginName}' from assembly '{assemblyName.Name} v{assemblyName.Version}' loaded.");
        
        return plugin;
    }
    
    public void StartPlugin(string pluginName)
    {
        var assembly = LoadAssemblyByPluginName(pluginName);
        var plugin = TryGetPlugin<IPlugin>(assembly, pluginName);
        
        pluginExecutor.Execute(plugin);
    }

    public void StartPlugins(IEnumerable<string> pluginNames)
    {
        foreach (var pluginName in pluginNames)
            StartPlugin(pluginName);
    }
    
    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        var metadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (metadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
        MetadataValidator.Validate(metadata);
        
        var plugins = metadata.Plugins.Select(x => x.Name).ToList();
        StartPlugins(plugins);
    }
    
    public void StartAllPlugins()
    {
        var assembliesPath = Directory.GetFiles(loader.GetPluginPath(), "*.dll");
        foreach (var assembly in assembliesPath)
            StartAllPluginsFromAssembly(Path.GetFileNameWithoutExtension(assembly));
    }
    
    
    public void StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        var assembly = LoadAssemblyByPluginName(pluginName);
        var extensionPlugin = TryGetPlugin<IExtensionPlugin<T>>(assembly, pluginName);
        
        pluginExecutor.ExecuteExtensionPlugin(ref data, extensionPlugin);
    }
    
    
    public byte[] ReceiveNetworkPlugin(string pluginName)
    {
        var assembly = LoadAssemblyByPluginName(pluginName);
        var networkPlugin = TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        
        return pluginExecutor.ExecuteNetworkPluginReceive(networkPlugin);
    }
    
    public void SendNetworkPlugin(string pluginName, byte[] data)
    {
        var assembly = LoadAssemblyByPluginName(pluginName);
        var networkPlugin = TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        
        pluginExecutor.ExecuteNetworkPluginSend(data, networkPlugin);
    }
}
