using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IPluginTracker : IPluginTrackerPublic, IMetadataRepositoryObserver
{
    void RegisterPlugin(PluginMetadata plugin);
    void RegisterPlugins(IEnumerable<PluginMetadata> plugins);
    void RemovePlugins(IEnumerable<string> pluginNames);
    void Clear();

    
    void SetPluginState(string pluginName, PluginState state);
    void SetPluginsState(IEnumerable<string> pluginNames, PluginState state);
}