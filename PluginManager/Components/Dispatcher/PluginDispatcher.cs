using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public class PluginDispatcher
{
    private readonly IAssemblyLoader _loader;
    
    public PluginMetadataDispatcher Metadata { get; }
    public PluginStartDispatcher Starter { get; }
    public PluginUnloadDispatcher Unloader { get; }
    

    public PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, 
        IAssemblyHandler handler, IPluginExecutor pluginExecutor, IPluginLifecycleManager lifecycleManager, 
        PluginLoggingFacade logger, IPluginLoaderService loaderService, IPluginMetadataService metadataService, 
        IDependencyResolverService dependencyResolver)
    {
        _loader = loader;
        
        Metadata = new(repository, metadataService, loader, handler, lifecycleManager, 
            logger);
        Starter = new(metadataService, loaderService, loader, pluginExecutor, 
            dependencyResolver, logger);
        Unloader = new(metadataService, loader, lifecycleManager);
    }
    
    public void ChangePluginDirectory(string pluginDirectory)
    { 
        _loader.ChangeSource(pluginDirectory);
        Metadata.RebuildMetadata();
    }
}