using PluginAPI;

namespace Plugins3;

public class ConsolePlugin2 : PluginBase, IExecutablePlugin
{
    public override string Name => "ConsolePlugin2";
    public override Version Version => new(1, 0, 1);
    public override string Author => "kand1s";

    public void Execute()
    {
        Console.WriteLine("Console plugin: initialization");
        Console.WriteLine("Console plugin: start executing");
        Console.WriteLine("Console plugin: finalizing");
    }
}