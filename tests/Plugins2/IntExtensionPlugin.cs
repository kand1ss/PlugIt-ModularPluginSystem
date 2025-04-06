using PluginAPI;

namespace Plugins2;

public class IntExtensionPlugin : PluginBase, IExtensionPlugin<int>
{
    public override string Name => "IntExtensionPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "IntExtensionPluginDescription";
    public override string Author => "kand1s";
    
    public void Expand(ref int data)
    {
        data *= 10;
        GetDependencyPlugin<INetworkPlugin>("NetworkPlugin").SendData([101, 43, 42]);
    }
}