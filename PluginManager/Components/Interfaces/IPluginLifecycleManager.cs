using ModularPluginAPI.Components.Lifecycle;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginLifecycleManager
{
    void SetPluginState(string pluginName, PluginState state);
    void SetPluginsState(IEnumerable<string> pluginNames, PluginState state);
    void SetAllPluginsUnloaded();
    
    void UnloadPlugin(string pluginName);
    void UnloadPlugins(IEnumerable<string> pluginNames);
    void Clear();

    IEnumerable<PluginInfo> GetLifecycleStatistics();
    PluginState GetPluginState(string plugin);  
}