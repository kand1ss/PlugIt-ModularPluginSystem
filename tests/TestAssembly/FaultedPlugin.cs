using PluginAPI;

namespace TestAssembly;

public class FaultedPlugin : PluginBase, IExecutablePlugin
{
    public override string Name => "FaultedPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Faulted plugin";
    public override string Author => "kand1s";
    
    public void Execute()
    {
        throw new Exception();
    }
}