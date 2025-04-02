using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Profiler;

public class PluginPerformanceProfiler : IPluginPerformanceProfiler
{
    private readonly Dictionary<string, ProfiledData> _profiledData = new();
    private readonly Dictionary<string, PluginStateDurationTimer> _timers = new();

    private readonly PluginProfilerLogger _logger = new();

    public void ExportProfilerLogs(ILogExporter exporter)
        => _logger.Export(exporter);

    public void OnPluginStateChanged(PluginInfo plugin)
    {
        CheckProfiledDataExists(plugin);
        SetValueFromTimer(plugin);

        if (CheckPluginIsCompleted(plugin)) 
            return;
        
        CreateNewTimer(plugin);
    }

    private void CheckProfiledDataExists(PluginInfo plugin)
    {
        if (!_profiledData.ContainsKey(plugin.Name))
            _profiledData[plugin.Name] = new ProfiledData { PluginName = plugin.Name };
    }

    private void SetValueFromTimer(PluginInfo plugin)
    {
        if (_timers.TryGetValue(plugin.Name, out var timer))
        {
            timer.StopTimer();
            TrySetInitializingTime(plugin, timer);
            TrySetExecutingTime(plugin, timer);
            TrySetFinalizingTime(plugin, timer);
            _timers.Remove(plugin.Name);
        }
    }

    private bool CheckPluginIsCompleted(PluginInfo plugin)
    {
        if (plugin.State == PluginState.Completed)
        {
            var data = _profiledData[plugin.Name];
            _logger.CreateLog(data);
            _profiledData.Remove(plugin.Name);
            return true;
        }

        return false;
    }

    private void CreateNewTimer(PluginInfo plugin)
    {
        var newTimer = new PluginStateDurationTimer();
        newTimer.PluginState = plugin.State;
        _timers.Add(plugin.Name, newTimer);
        newTimer.StartTimer();
    }

    private void TrySetInitializingTime(PluginInfo plugin, PluginStateDurationTimer timer)
    {
        if (timer.PluginState == PluginState.Initializing)
        {
            _profiledData[plugin.Name].ItWasInitialized = true;
            _profiledData[plugin.Name].InitializingTimeMs = timer.GetDuration();
        }
    }

    private void TrySetExecutingTime(PluginInfo plugin, PluginStateDurationTimer timer)
    {
        if (timer.PluginState == PluginState.Running)
        {
            _profiledData[plugin.Name].ItWasExecuted = true;
            _profiledData[plugin.Name].ExecutingTimeMs = timer.GetDuration();
        }
    }

    private void TrySetFinalizingTime(PluginInfo plugin, PluginStateDurationTimer timer)
    {
        if (timer.PluginState == PluginState.Finalizing)
        {
            _profiledData[plugin.Name].ItWasFinalized = true;
            _profiledData[plugin.Name].FinalizingTimeMs = timer.GetDuration();
        }
    }
}