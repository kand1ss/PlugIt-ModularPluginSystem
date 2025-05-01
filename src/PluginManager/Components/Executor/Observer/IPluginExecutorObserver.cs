using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.Observer;

public interface IPluginExecutorObserver
{
    void OnPluginStatusChanged(PluginStatus plugin);
}