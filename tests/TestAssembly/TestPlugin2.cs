using PluginAPI;

namespace TestAssembly;

public class TestPlugin2 : PluginBase, IExecutablePlugin
{
    public override string Name => "TestPlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Test plugin";
    public override string Author => "kand1ss";

    public void Execute()
    {
        Console.WriteLine("TestPlugin2 initialized");
        Console.WriteLine("TestPlugin2 executed");
    }
}