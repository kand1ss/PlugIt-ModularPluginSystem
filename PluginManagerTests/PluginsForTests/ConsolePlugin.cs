using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class ConsolePlugin : PluginBase
{
    public override string Name => "ConsolePlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";
    
    
    public override void Initialize()
    {
        Console.WriteLine("Console plugin initialized");
    }

    public override void Execute()
    {
        Console.WriteLine("Console plugin executed");
    }

    public override void FinalizePlugin()
    {
        Console.WriteLine("Console plugin finalized");
    }
}