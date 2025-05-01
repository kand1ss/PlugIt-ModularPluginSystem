using PluginAPI;

namespace TestAssembly;

public class UnsafePlugin : PluginBase, IExecutablePlugin
{
    public override string Name => "UnsafePlugin";
    public override Version Version => new(1, 0, 0);
    public override string Author => "kand1s";
    
    
    public void Execute()
    {
        Console.WriteLine(File.Exists(Path.Combine("D", "SomePath")));
    }
}