using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;

namespace PluginManagerTests;

public class StateObserver : IPluginExecutorObserver
{
    public List<PluginState> ReceivedStates = new();

    public void OnPluginStatusChanged(PluginStatus plugin)
        => ReceivedStates.Add(plugin.CurrentState);
}