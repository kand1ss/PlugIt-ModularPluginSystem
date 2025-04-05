using System.Diagnostics;
using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.Profiler;

public class PluginStateDurationTimer
{
    public PluginState PluginState { get; set; }
    private readonly Stopwatch _timer = new();
    
    public void StartTimer()
        => _timer.Start();
    public void StopTimer()
        => _timer.Stop();
    public long GetDuration()
    {
        StopTimer();
        return _timer.ElapsedMilliseconds;
    }
}