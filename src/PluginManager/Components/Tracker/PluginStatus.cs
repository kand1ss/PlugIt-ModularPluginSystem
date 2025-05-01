namespace ModularPluginAPI.Components.Lifecycle;

public class PluginStatus
{
    public string Name { get; init; } = "";
    public Version Version { get; init; } = new(0,0);
    public string Description { get; init; } = "";
    public string Author { get; init; } = "";
    
    public PluginState CurrentState { get; set; }
    public PluginMode CurrentMode { get; set; }
}