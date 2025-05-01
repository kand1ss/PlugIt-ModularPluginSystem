using System.Diagnostics;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger.Interfaces;
using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components.Profiler;

public class PluginPerformanceProfiler : IPluginPerformanceProfiler, 
    IPluginExecutorObserver, IErrorHandledPluginExecutorObserver
{
    private readonly Dictionary<string, ProfiledData> _profiledData = new();
    private readonly Dictionary<string, Stopwatch> _timers = new();

    private readonly PluginProfilerLogger _logger = new();

    public void ExportProfilerLogs(ILogExporter exporter)
        => _logger.Export(exporter);

    private void CompleteDataAndCreateLog(PluginStatus plugin)
    {
        if (!_profiledData.TryGetValue(plugin.Name, out var data))
            return;
        
        _logger.CreateLog(data);
        _profiledData.Remove(plugin.Name);
    }
    
    public void OnPluginFaulted(PluginStatus plugin, Exception exception)
    {
        SetValueFromTimer(plugin);
        CompleteDataAndCreateLog(plugin);
    }

    public void OnPluginStatusChanged(PluginStatus plugin)
    {
        CheckProfiledDataExists(plugin);
        SetValueFromTimer(plugin);

        if (plugin.CurrentState == PluginState.Completed)
        {
            CompleteDataAndCreateLog(plugin);
            return;
        }
        
        CreateNewTimer(plugin);
    }

    private void CheckProfiledDataExists(PluginStatus plugin)
    {
        if (!_profiledData.ContainsKey(plugin.Name))
            _profiledData[plugin.Name] = new ProfiledData
            {
                PluginName = plugin.Name,
                PluginMode = plugin.CurrentMode
            };
    }

    private void SetValueFromTimer(PluginStatus plugin)
    {
        SetExecutingTime(plugin);
        _timers.Remove(plugin.Name);
    }

    private void CreateNewTimer(PluginStatus plugin)
    {
        var newTimer = new Stopwatch();
        _timers.Add(plugin.Name, newTimer);
        newTimer.Start();
    }

    private void SetExecutingTime(PluginStatus plugin)
    {
        if (!_timers.TryGetValue(plugin.Name, out var timer))
            return;
        
        timer.Stop();
        _profiledData[plugin.Name].ItWasExecuted = true;
        _profiledData[plugin.Name].ExecutingTimeMs = timer.ElapsedMilliseconds;
    }
}