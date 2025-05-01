using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.Profiler;

public class ProfiledData
{
    public string PluginName { get; set; } = "";
    public PluginMode PluginMode { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    
    public long ExecutingTimeMs { get; set; } = 0;
    public bool ItWasExecuted { get; set; }
}