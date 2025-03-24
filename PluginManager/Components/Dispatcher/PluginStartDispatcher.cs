using System.Collections;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
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

    private IEnumerable<string> GetPluginNamesFromAssembly(string assemblyName)
    {
        var metadata = metadataService.GetMetadata(assemblyName);
        MetadataValidator.Validate(metadata);
        return metadataService.GetPluginNamesFromMetadata(metadata);
    }

    
    public ExecutionResult StartPlugin(string pluginName)
    {
        var plugin = GetPluginFromAssembly<IPlugin>(pluginName);
        return pluginExecutor.Execute(plugin);
    }

    public ExecutionResult StartPlugins(IEnumerable<string> pluginNames)
    {
        var executionResults = new List<ExecutionResult>();
        foreach (var pluginName in pluginNames)
            executionResults.Add(StartPlugin(pluginName));
        
        return ExecutionResult.FromResults(executionResults);
    }
    
    public ExecutionResult StartAllPluginsFromAssembly(string assemblyName)
    {
        var plugins = GetPluginNamesFromAssembly(assemblyName);
        return StartPlugins(plugins);
    }
    
    public ExecutionResult StartAllPlugins()
    {
        var executionResults = new List<ExecutionResult>();
        foreach (var assembly in loader.GetAllAssembliesNames())
            executionResults.AddRange(StartAllPluginsFromAssembly(assembly));
        
        return ExecutionResult.FromResults(executionResults);
    }
    
    public ExecutionResult StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        var plugin = GetPluginFromAssembly<IExtensionPlugin<T>>(pluginName);
        return pluginExecutor.ExecuteExtensionPlugin(ref data, plugin);
    }
    
    
    public ExecutionResult ReceiveNetworkPlugin(string pluginName, out byte[] response)
    {
        response = [];
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        return pluginExecutor.ExecuteNetworkPluginReceive(plugin, out response);
    }
    
    public ExecutionResult SendNetworkPlugin(string pluginName, byte[] data)
    {
        var plugin = GetPluginFromAssembly<INetworkPlugin>(pluginName);
        return pluginExecutor.ExecuteNetworkPluginSend(data, plugin);
    }
}