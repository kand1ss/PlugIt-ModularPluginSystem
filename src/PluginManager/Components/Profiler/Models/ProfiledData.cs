namespace ModularPluginAPI.Components.Profiler;

public class ProfiledData
{
    public string PluginName { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    
    public long InitializingTimeMs { get; set; } = 0;
    public long ExecutingTimeMs { get; set; } = 0;
    public long FinalizingTimeMs { get; set; } = 0;

    public bool ItWasInitialized { get; set; }
    public bool ItWasExecuted { get; set; }
    public bool ItWasFinalized { get; set; }
}