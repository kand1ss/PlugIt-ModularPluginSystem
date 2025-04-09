using PluginAPI;

namespace PluginManagerTests.Test_Plugins;

public class TestPluginWithExtension : IExecutablePlugin
{
    public string Name { get; } = string.Empty;
    public Version Version { get; } = new(1, 0, 0);
    public string Description { get; } = string.Empty;
    public string Author { get; } = string.Empty;
    
    public void Execute()
    {
        throw new Exception();
    }
}