using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Lifecycle.Observer;

namespace ConsoleTest;

public class Observer : IPluginTrackerObserver
{
    public void OnPluginRegistered(PluginInfo plugin)
    {
        Console.WriteLine($"Registered plugin: {plugin.Name}");
    }

    public void OnPluginRemoved(PluginInfo plugin)
    {
        Console.WriteLine($"Removed plugin: {plugin.Name}");
    }

    public void OnPluginStateChanged(PluginInfo plugin)
    {
        Console.WriteLine($"State changed: {plugin.Name} - {plugin.State}");
    }
}