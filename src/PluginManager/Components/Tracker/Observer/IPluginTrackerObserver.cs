namespace ModularPluginAPI.Components.Lifecycle.Observer;

public interface IPluginTrackerObserver
{
    void OnPluginRegistered(PluginStatus plugin);
    void OnPluginRemoved(PluginStatus plugin);
    void OnPluginStatusChanged(PluginStatus plugin);
}