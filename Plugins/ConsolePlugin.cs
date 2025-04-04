using PluginAPI;

namespace Plugins;

public class ConsolePlugin : PluginBase, IInitialisablePlugin, IExecutablePlugin, IFinalisablePlugin
{
    public override string Name => "ConsolePlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Console plugin";
    public override string Author => "kand1s";

    public void Initialize()
    {
        Console.WriteLine("Console plugin: initialization");
    }

    public void Execute()
    {
        var data = "somedata";
        Console.WriteLine("Console plugin: start executing");
        GetDependencyPlugin<IExtensionPlugin<string>>("StringExtensionPlugin").Expand(ref data);
        Console.WriteLine($"Console plugin: end executing {data}");
        var data2 = 12;
        GetDependencyPlugin<IExtensionPlugin<int>>("IntExtensionPlugin").Expand(ref data2);
        Console.WriteLine($"Console plugin: end executing {data2}");
    }
    
    public void FinalizePlugin()
    {
        Console.WriteLine("Console plugin: finalizing");
    }
}