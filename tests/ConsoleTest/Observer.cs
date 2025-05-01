using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Lifecycle.Observer;

namespace ConsoleTest;

public class Observer : IPluginTrackerObserver
{
    public void OnPluginRegistered(PluginStatus plugin)
    {
        Console.WriteLine($"Registered plugin: {plugin.Name}");
    }

    public void OnPluginRemoved(PluginStatus plugin)
    {
        Console.WriteLine($"Removed plugin: {plugin.Name}");
    }

    public void OnPluginStatusChanged(PluginStatus plugin)
    {
        Console.WriteLine($"State changed: {plugin.Name} - {plugin.CurrentState}");
    }
}