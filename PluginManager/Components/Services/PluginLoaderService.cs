using System.Reflection;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginLoaderService(PluginMetadataService metadataService, IAssemblyLoader loader, 
    IAssemblyHandler handler, PluginLoggingFacade logger) : IPluginLoaderService
{
    public Assembly LoadAssembly(string assemblyName)
        => loader.LoadAssembly(assemblyName)
            ?? throw new AssemblyNotFoundException(assemblyName);
    
    public Assembly LoadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        return loader.LoadAssembly(metadata.Path);
    }

    public T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
        => handler.GetPlugin<T>(assembly, pluginName)
           ?? throw new PluginNotFoundException(pluginName);
}