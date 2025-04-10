using PluginAPI;

namespace TestAssembly;

public class FaultedExtensionPlugin : PluginBase, IExtensionPlugin<string>
{
    public override string Name => "ExtensionFaultedPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Faulted plugin";
    public override string Author => "kand1s";

    public void Expand(ref string data)
    {
        throw new Exception();
    }
}