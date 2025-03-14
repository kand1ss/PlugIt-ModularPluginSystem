using System.Reflection;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginDispatcher
{
    public PluginMetadataDispatcher Metadata { get; }
    public PluginStartDispatcher Starter { get; }
    public PluginUnloadDispatcher Unloader { get; }
    

    public PluginDispatcher(IAssemblyMetadataRepository repository, IAssemblyLoader loader, 
        IAssemblyHandler handler, IPluginExecutor pluginExecutor, IPluginLifecycleManager lifecycleManager, 
        ILoggerService logger)
    {
        var metadataGenerator = new AssemblyMetadataGenerator(handler);
        
        Metadata = new
            (repository, loader, lifecycleManager, metadataGenerator, logger);
        Starter = new(repository, loader, handler, pluginExecutor, logger);
        Unloader = new(loader, repository, lifecycleManager);
    }
}