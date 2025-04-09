using PluginAPI;

namespace TestAssembly;

public class TestPlugin : PluginBase, IInitialisablePlugin, IExecutablePlugin
{
    public override string Name => "TestPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Test plugin";
    public override string Author => "kand1ss";
    
    public void Initialize()
    {
        Console.WriteLine("TestPlugin initialized");
    }

    public void Execute()
    {
        Console.WriteLine("TestPlugin executed");
    }
}