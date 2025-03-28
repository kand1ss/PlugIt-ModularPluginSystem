using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginTracker
{
    void RegisterPlugin(PluginMetadata plugin);
    void RegisterPlugins(IEnumerable<PluginMetadata> plugins);
    void RemovePlugins(IEnumerable<string> pluginNames);
    void Clear();

    
    void SetPluginState(string pluginName, PluginState state);
    void SetPluginsState(IEnumerable<string> pluginNames, PluginState state);


    IEnumerable<PluginInfo> GetPluginsStatus();
    PluginInfo GetPluginStatus(string plugin);  
}