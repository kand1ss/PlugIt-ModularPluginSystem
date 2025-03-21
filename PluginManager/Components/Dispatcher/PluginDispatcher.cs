using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public class PluginDispatcher
{
    public PluginMetadataDispatcher Metadata { get; }
    public PluginStartDispatcher Starter { get; }
    public PluginUnloadDispatcher Unloader { get; }
    

    public PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, 
        IAssemblyHandler handler, IPluginExecutor pluginExecutor, IPluginLifecycleManager lifecycleManager, 
        PluginLoggingFacade logger, IPluginDependencyResolver dependencyResolver)
    {
        var metadataService = new PluginMetadataService(repository);
        var loaderService = new PluginLoaderService(metadataService, loader, handler, logger);
        
        Metadata = new(repository, metadataService, loader, handler, lifecycleManager, 
            logger);
        Starter = new(metadataService, loaderService, loader, pluginExecutor, 
            dependencyResolver);
        Unloader = new(metadataService, loader, lifecycleManager);
    }
}