using ModularPluginAPI.Components.AssemblyWatcher.Interfaces;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public class PluginDispatcher
{
    private readonly IAssemblyWatcher _assemblyWatcher;
    
    public PluginMetadataDispatcher Metadata { get; }
    public PluginStartDispatcher Starter { get; }
    public PluginUnloadDispatcher Unloader { get; }
    

    public PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, 
        IAssemblyHandler handler, IPluginExecutor pluginExecutor, IPluginTracker tracker, 
        PluginLoggingFacade logger, IPluginLoaderService loaderService, IPluginMetadataService metadataService, 
        IDependencyResolverService dependencyResolver, IAssemblyWatcher assemblyWatcher)
    {
        Metadata = new(repository, metadataService, loader, handler, logger);
        _assemblyWatcher = assemblyWatcher;
        _assemblyWatcher.AddObserver(Metadata);
        
        Starter = new(metadataService, loaderService, pluginExecutor, 
            dependencyResolver, logger);
        Unloader = new(metadataService, loader, tracker);
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