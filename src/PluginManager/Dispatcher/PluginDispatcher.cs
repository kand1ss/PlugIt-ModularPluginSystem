using ModularPluginAPI.Components.AssemblyWatcher.Interfaces;
using ModularPluginAPI.Components.AssemblyWatcher.Observer;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Plugin_Configurator.Interfaces;

namespace ModularPluginAPI.Components;

public class PluginDispatcher : IAssemblyWatcherObserver
{
    private readonly IAssemblyWatcher _assemblyWatcher;
    
    public PluginMetadataDispatcher Metadata { get; }
    public PluginStartDispatcher Starter { get; }
    public PluginUnloadDispatcher Unloader { get; }
    

    public PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, 
        IAssemblyHandler handler, IPluginExecutor pluginExecutor, IPluginTracker tracker, 
        PluginLoggingFacade logger, IPluginLoaderService loaderService, IPluginMetadataService metadataService, 
        IPluginConfiguratorService configuratorService, IAssemblyWatcher assemblyWatcher)
    {
        Metadata = new(repository, metadataService, loader, handler, logger);
        _assemblyWatcher = assemblyWatcher;
        
        Starter = new(metadataService, loaderService, pluginExecutor, configuratorService, logger);
        Unloader = new(metadataService, loaderService);
    }
    
    
    public void OnAssemblyAdded(string assemblyPath)
        => CreateMetadata(assemblyPath);

    public void OnAssemblyRemoved(string assemblyPath)
        => Metadata.RemoveMetadata(assemblyPath);

    public void OnAssemblyChanged(string assemblyPath)
    {
        Metadata.RemoveMetadata(assemblyPath);
        CreateMetadata(assemblyPath);
    }
    
    

    public void CreateMetadata(string assemblyPath)
    {
        Metadata.LoadMetadata(assemblyPath);
        Unloader.UnloadAssembly(assemblyPath);
    }

    public void RegisterAssembly(string assemblyPath)
        => _assemblyWatcher.ObserveAssembly(assemblyPath);

    public void RegisterAssembliesFromDirectory(string directoryPath)
    {
        var assemblies = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach (var assembly in assemblies)
            RegisterAssembly(assembly);
    }
    
    public void UnregisterAssembly(string assemblyPath)
        => _assemblyWatcher.UnobserveAssembly(assemblyPath);

    public void UnregisterAssembliesFromDirectory(string directoryPath)
    {
        var assemblies = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach (var assembly in assemblies)
            UnregisterAssembly(assembly);
    }
}