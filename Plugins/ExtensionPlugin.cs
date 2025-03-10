using PluginAPI;

namespace Plugins;

public class ExtensionPlugin : IExtensionPlugin<string>
{
    public string Name => "StringExtensionPlugin";

    public Version Version => new(1, 0, 0);

    public string Description => "String extension plugin";
    
    public void Expand(ref string data)
    {
        data = data.ToUpper();
        data = "[" + data + "]";
    }
}