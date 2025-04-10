using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components;

public interface IPluginTrackerPublic : IObservablePluginTracker
{
    /// <summary>
    /// Retrieves the states of all plugins.
    /// </summary>
    /// <returns>An enumerable collection of PluginInfo objects representing the states of all plugins.</returns>
    IEnumerable<PluginInfo> GetPluginsStatus();
    
    /// <summary>
    /// Retrieves the state of a specific plugin by its name.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose state is to be retrieved.</param>
    /// <returns>The current state of the specified plugin as a string.</returns>
    PluginInfo? GetPluginStatus(string pluginName);  
}