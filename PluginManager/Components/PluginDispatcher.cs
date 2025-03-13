using System.Reflection;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, IAssemblyHandler handler, 
    IPluginExecutor pluginExecutor, IPluginLifecycleManager lifecycleManager)
{
    private readonly AssemblyMetadataGenerator _metadataGenerator = new(handler);

    private AssemblyMetadata TryGetMetadata(string assemblyName)
    {
        var assemblyMetadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (assemblyMetadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
        MetadataValidator.Validate(assemblyMetadata);
        return assemblyMetadata;
    }
    private AssemblyMetadata TryGetMetadataByPluginName(string pluginName)
    {
        var assemblyMetadata = repository.GetMetadataByPluginName(pluginName);
        if (assemblyMetadata is null)
            throw new PluginNotFoundException(pluginName);
        
        MetadataValidator.Validate(assemblyMetadata);
        return assemblyMetadata;
    }
    private Assembly LoadAssembly(string assemblyName)
    {
        var metadata = TryGetMetadata(assemblyName);
        var pluginNames = metadata.Plugins.Select(x => x.Name).ToList();
        
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Loaded);
        return loader.GetAssembly(assemblyName);
    }
    private Assembly LoadAssemblyByPluginName(string pluginName)
    {
        var assemblyMetadata = TryGetMetadataByPluginName(pluginName);
        return LoadAssembly(assemblyMetadata.Name);
    }
    private T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
    {
        var plugin = handler.GetPlugin<T>(assembly, pluginName);
        if(plugin is null)
            throw new PluginNotFoundException(pluginName);
        
        return plugin;
    }


    public void RebuildMetadata()
    {
        repository.Clear();
        lifecycleManager.Clear();
        
        var assemblies = loader.GetAllAssemblies();
        var assemblyMetadata = _metadataGenerator.Generate(assemblies).ToList();
        
        repository.AddRange(assemblyMetadata);
        UnloadAllAssemblies();
    }
    
    public void StartPlugin(string pluginName)
    {
        var assembly = LoadAssemblyByPluginName(pluginName);
        var plugin = TryGetPlugin<IPlugin>(assembly, pluginName);
        
        pluginExecutor.Execute(plugin);
    }
    public void StartPlugins(IEnumerable<string> pluginNames)
        => pluginNames.ToList().ForEach(StartPlugin);
    
    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        var assembly = LoadAssembly(assemblyName);
        var plugins = handler.GetAllPlugins(assembly);
        
        pluginExecutor.Execute(plugins);
    }
    public void StartAllPlugins()
    {
        var assemblies = loader.GetAllAssemblies();
        var plugins = handler.GetAllPlugins(assemblies).ToList();
        var pluginNames = plugins.Select(p => p.Name);
        
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Loaded);
        pluginExecutor.Execute(plugins);
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


    public void UnloadAssembly(string assemblyName)
    {
        var assemblyMetadata = TryGetMetadata(assemblyName);
        var plugins = assemblyMetadata.Plugins;
        var pluginNames = plugins.Select(p => p.Name);
        
        lifecycleManager.SetPluginsState(pluginNames, PluginState.Unloaded);
        loader.UnloadAssembly(assemblyName);
    }

    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var assembly = TryGetMetadataByPluginName(pluginName);
        UnloadAssembly(assembly.Name);
    }

    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.ToList().ForEach(UnloadAssembly);
    
    public void UnloadAllAssemblies()
    {
        lifecycleManager.SetAllPluginsUnloaded();
        loader.UnloadAllAssemblies();
    }


    public IEnumerable<PluginInfo> GetPluginStates()
        => lifecycleManager.GetLifecycleStatistics();

    public string GetPluginState(string pluginName)
        => lifecycleManager.GetPluginState(pluginName).ToString();
}