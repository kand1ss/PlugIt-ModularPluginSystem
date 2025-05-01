using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IPluginTracker : IPluginTrackerPublic
{
    void RegisterPlugin(PluginMetadata plugin);
    void RegisterPlugins(IEnumerable<PluginMetadata> plugins);
    void RemovePlugins(IEnumerable<string> pluginNames);
    void Clear();

    
    void SetPluginStatus(string pluginName, PluginState state, PluginMode mode);
    void SetPluginsStatus(IEnumerable<string> pluginNames, PluginState state, PluginMode mode);
}