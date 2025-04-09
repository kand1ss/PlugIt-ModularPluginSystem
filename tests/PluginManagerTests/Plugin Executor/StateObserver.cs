using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;

namespace PluginManagerTests;

public class StateObserver : IPluginExecutorObserver
{
    public List<PluginState> ReceivedStates = new();

    public void OnPluginStateChanged(PluginInfo plugin)
        => ReceivedStates.Add(plugin.State);
}