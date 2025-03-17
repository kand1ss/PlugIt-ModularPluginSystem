using PluginAPI;

namespace Plugins2;

public class ConsolePlugin4 : PluginBase, IExecutablePlugin
{
    public override string Name => "ConsolePlugin4";
    public override Version Version => new(1, 0, 2);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";

    public override void Initialize()
    {
        Console.WriteLine("Console plugin: initialization");
    }

    public void Execute()
    {
        Console.WriteLine("Console plugin: start executing");
    }
    
    public override void FinalizePlugin()
    {
        Console.WriteLine("Console plugin: finalizing");
    }
}