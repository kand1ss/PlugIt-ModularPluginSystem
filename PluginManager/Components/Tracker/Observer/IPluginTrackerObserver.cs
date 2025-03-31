namespace ModularPluginAPI.Components.Lifecycle.Observer;

public interface IPluginTrackerObserver
{
    void OnPluginRegistered(PluginInfo plugin);
    void OnPluginRemoved(PluginInfo plugin);
    void OnPluginStateChanged(PluginInfo plugin);
}