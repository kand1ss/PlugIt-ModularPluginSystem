using PluginAPI;

namespace TestAssembly;

public class TestPlugin : PluginBase,  IExecutablePlugin
{
    public override string Name => "TestPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Author => "kand1ss";

    public void Execute()
    {
        Console.WriteLine("TestPlugin initialized");
        Console.WriteLine("TestPlugin executed");
    }
}