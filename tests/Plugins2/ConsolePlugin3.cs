using PluginAPI;

namespace Plugins2;

public class ConsolePlugin3 : PluginBase, IInitialisablePlugin, IExecutablePlugin, IFinalisablePlugin
{
    public override string Name => "ConsolePlugin3";
    public override Version Version => new(1, 0, 2);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";

    public void Initialize()
    {
        Console.WriteLine("Console plugin: initialization");
    }

    public void Execute()
    {
        Console.WriteLine("Console plugin: start executing");
    }
    
    public void FinalizePlugin()
    {
        Console.WriteLine("Console plugin: finalizing");
    }
}