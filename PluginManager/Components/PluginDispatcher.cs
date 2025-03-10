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
        
        return assemblyMetadata;
    }
    


    public void RebuildMetadata()
    {
        repository.Clear();
        var assemblies = loader.GetAllAssemblies();
        var metadata = assemblies.Select(_metadataGenerator.Generate);
        
        repository.AddRange(metadata);
        loader.UnloadAllAssemblies();
    }
    
    
    public void StartPlugin(string pluginName)
    {
        var assemblyMetadata = TryGetMetadataByPluginName(pluginName);
        var assembly = loader.GetAssembly(assemblyMetadata.Name);
        var plugin = handler.GetPlugin(assembly, pluginName);
        
        pluginExecutor.Execute(plugin!);
    }

    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        var metadata = repository.GetMetadataByAssemblyName(assemblyName);
        if (metadata is null)
            throw new AssemblyNotFoundException(assemblyName);
        
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
        var metadata = TryGetMetadataByPluginName(pluginName);
        var assembly = loader.GetAssembly(metadata.Name);
        var extensionPlugin = handler.GetPlugin<IExtensionPlugin<T>>(assembly, pluginName);
        
        if (extensionPlugin is null)
            throw new PluginNotFoundException(pluginName);
        
        pluginExecutor.ExecuteExtensionPlugin(ref data, extensionPlugin);
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