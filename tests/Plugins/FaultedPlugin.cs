using PluginAPI;

namespace Plugins;

public class FaultedPlugin : PluginBase, IExecutablePlugin
{
    public override string Name => "FaultedPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Author => "kand1ss";
    
    
    public void Execute()
    {
        throw new Exception("Faulted plugin");
    }
}