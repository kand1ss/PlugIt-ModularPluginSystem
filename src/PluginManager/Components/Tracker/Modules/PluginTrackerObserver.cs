using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle.Modules;

public class PluginTrackerObserver(IPluginTracker tracker) :
    IErrorHandledPluginExecutorObserver, IMetadataRepositoryObserver, IPluginExecutorObserver, ILoaderServiceObserver
{
    public void OnMetadataAdded(AssemblyMetadata assemblyMetadata)
    {
        var plugins = assemblyMetadata.Plugins;
        tracker.RegisterPlugins(plugins);
    }

    public void OnMetadataRemoved(AssemblyMetadata assemblyMetadata)
    {
        var pluginNames = assemblyMetadata.Plugins.Select(x => x.Name);
        tracker.RemovePlugins(pluginNames);
    }
    
    public void OnAssemblyLoaded(AssemblyMetadata assemblyMetadata)
    {
        var pluginNames = assemblyMetadata.Plugins.Select(x => x.Name);
        tracker.SetPluginsStatus(pluginNames, PluginState.Loaded, PluginMode.Idle);
    }

    public void OnAssemblyUnloaded(AssemblyMetadata assemblyMetadata)
    {
        var pluginNames = assemblyMetadata.Plugins.Select(x => x.Name);
        tracker.SetPluginsStatus(pluginNames, PluginState.Unloaded, PluginMode.Idle);
    }

    public void OnPluginStatusChanged(PluginStatus plugin)
        => tracker.SetPluginStatus(plugin.Name, plugin.CurrentState, plugin.CurrentMode);
    public void OnPluginFaulted(PluginStatus plugin, Exception exception)
        => tracker.SetPluginStatus(plugin.Name, PluginState.Faulted, plugin.CurrentMode);

}