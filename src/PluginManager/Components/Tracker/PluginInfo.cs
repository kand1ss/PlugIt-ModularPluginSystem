namespace ModularPluginAPI.Components.Lifecycle;

public class PluginInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public PluginState State { get; set; }
}