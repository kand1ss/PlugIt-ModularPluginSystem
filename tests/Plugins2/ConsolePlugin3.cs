using PluginAPI;

namespace Plugins2;

public class ConsolePlugin3 : PluginBase, IExecutablePlugin
{
    public override string Name => "ConsolePlugin3";
    public override Version Version => new(1, 0, 2);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";

    public void Execute()
    {
        Console.WriteLine("Console plugin: initialization");
        Console.WriteLine("Console plugin: start executing");
        Console.WriteLine("Console plugin: finalizing");
    }
}