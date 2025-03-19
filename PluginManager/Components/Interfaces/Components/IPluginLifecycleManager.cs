using ModularPluginAPI.Components.Lifecycle;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginLifecycleManager
{
    void SetPluginState(string pluginName, PluginState state);
    void SetPluginsState(IEnumerable<string> pluginNames, PluginState state);

    void RemovePlugins(IEnumerable<string> pluginNames);
    void Clear();

    IEnumerable<PluginInfo> GetPluginStates();
    PluginInfo GetPluginState(string plugin);  
}