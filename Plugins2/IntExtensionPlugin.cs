using PluginAPI;

namespace Plugins2;

public class IntExtensionPlugin : IExtensionPlugin<int>
{
    public string Name => "IntExtensionPlugin";
    public Version Version => new(1, 0, 0);
    public string Description => "IntExtensionPluginDescription";
    public string Author => "kand1s";

    public void Expand(ref int data)
    {
        data *= 10;
    }
}