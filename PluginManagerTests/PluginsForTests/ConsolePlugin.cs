using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class ConsolePlugin : PluginBase
{
    public override string Name => "ConsolePlugin";
    public override string Version => "1.0";
    public override string Description => "Console plugin";
    
    
    public override Task Initialize()
    {
        Console.WriteLine("Console plugin initialized");
        return Task.CompletedTask;
    }

    public override Task ExecuteAsync()
    {
        Console.WriteLine("Console plugin executed");
        return Task.CompletedTask;
    }

    public override Task FinalizeAsync()
    {
        Console.WriteLine("Console plugin finalized");
        return Task.CompletedTask;
    }
}