using System.Reflection;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginLoaderService(PluginMetadataService metadataService, IAssemblyLoader loader, 
    IAssemblyHandler handler, PluginLoggerLayer logger) : IPluginLoaderService
{
    public Assembly LoadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        return loader.LoadAssembly(metadata.Name);
    }

    private T LoadPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
        => handler.GetPlugin<T>(assembly, pluginName)
           ?? throw new PluginNotFoundException(pluginName);

    public T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
    {
        var plugin = LoadPlugin<T>(assembly, pluginName);
        
        var assemblyName = assembly.GetName().Name ?? "null";
        var assemblyVersion = assembly.GetName().Version ?? new Version(0, 0, 0);
        
        logger.PluginLoaded(plugin.Name, assemblyName, assemblyVersion);
        return plugin;
    }
}