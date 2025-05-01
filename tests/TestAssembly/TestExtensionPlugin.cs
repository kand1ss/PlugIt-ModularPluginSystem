using PluginAPI;

namespace PluginManagerTests.Test_Plugins;

public class TestExtensionPlugin : IExtensionPlugin<string>
{
    public string Name { get; } = string.Empty;
    public Version Version { get; } = new(1, 0, 0);
    public string Description { get; } = string.Empty;
    public string Author { get; } = string.Empty;
    
    public bool IsExecuted { get; private set; } = false;
    public void Expand(ref string data)
        => IsExecuted = true;
}