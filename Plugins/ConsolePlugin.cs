using PluginAPI;

namespace Plugins;

public class ConsolePlugin : PluginBase
{
    public override string Name => "ConsolePlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";
    
    
    public override void Initialize()
    {
        Console.WriteLine("Console plugin: initialization");
    }

    public override void Execute()
    {
        Console.WriteLine("Console plugin: executing");
        Thread.Sleep(50);
    }

    public override void FinalizePlugin()
    {
        Console.WriteLine("Console plugin: finalizing");
    }
}