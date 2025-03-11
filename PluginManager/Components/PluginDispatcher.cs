using System.Reflection;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, IAssemblyHandler handler, 
    IPluginExecutor pluginExecutor)
{
    private readonly AssemblyMetadataGenerator _metadataGenerator = new(handler);
    
    
    private AssemblyMetadata TryGetMetadataByPluginName(string pluginName)
    {
        var assemblyMetadata = repository.GetMetadataByPluginName(pluginName);
        if (assemblyMetadata is null)
            throw new PluginNotFoundException(pluginName);
        
        MetadataValidator.Validate(assemblyMetadata);
        return assemblyMetadata;
    }
    private AssemblyMetadata TryGetMetadataByAssemblyName(string assemblyName)
    {
        var assemblyMetadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (assemblyMetadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
        MetadataValidator.Validate(assemblyMetadata);
        return assemblyMetadata;
    }
    private Assembly GetAssemblyByPluginName(string pluginName)
    {
        var assemblyMetadata = TryGetMetadataByPluginName(pluginName);
        return loader.GetAssembly(assemblyMetadata.Name);
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
        var assemblies = loader.GetAllAssemblies();
        var metadata = _metadataGenerator.Generate(assemblies);
        
        repository.AddRange(metadata);
        loader.UnloadAllAssemblies();
    }
    
    
    public void StartPlugin(string pluginName)
    {
        var assembly = GetAssemblyByPluginName(pluginName);
        var plugin = TryGetPlugin<IPlugin>(assembly, pluginName);
        
        pluginExecutor.Execute(plugin);
    }
    public void StartPlugins(IEnumerable<string> pluginNames)
        => pluginNames.ToList().ForEach(StartPlugin);
    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        var metadata = TryGetMetadataByAssemblyName(assemblyName);
        var assembly = loader.GetAssembly(metadata.Name);
        var plugins = handler.GetAllPlugins(assembly);
        
        pluginExecutor.Execute(plugins);
    }
    public void StartAllPlugins()
    {
        var assemblies = loader.GetAllAssemblies();
        var plugins = handler.GetAllPlugins(assemblies);
        
        pluginExecutor.Execute(plugins);
    }
    
    
    public void StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        var assembly = GetAssemblyByPluginName(pluginName);
        var extensionPlugin = TryGetPlugin<IExtensionPlugin<T>>(assembly, pluginName);
        
        pluginExecutor.ExecuteExtensionPlugin(ref data, extensionPlugin);
    }

    public byte[] ReceiveNetworkPlugin(string pluginName)
    {
        var assembly = GetAssemblyByPluginName(pluginName);
        var networkPlugin = TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        
        return pluginExecutor.ExecuteNetworkPluginReceive(networkPlugin);
    }

    public void SendNetworkPlugin(string pluginName, byte[] data)
    {
        var assembly = GetAssemblyByPluginName(pluginName);
        var networkPlugin = TryGetPlugin<INetworkPlugin>(assembly, pluginName);
        
        networkPlugin.SendData(data);
    }


    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var assembly = TryGetMetadataByPluginName(pluginName);
        loader.UnloadAssembly(assembly.Name);
    }
    
    public void UnloadAssembly(string assemblyName)
        => loader.UnloadAssembly(assemblyName);

    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.ToList().ForEach(UnloadAssembly);
    
    public void UnloadAllAssemblies()
        => loader.UnloadAllAssemblies();
}