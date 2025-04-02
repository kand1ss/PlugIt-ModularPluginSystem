using ModularPluginAPI.Components.Logger.Interfaces;
using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components;

public interface IPluginPerformanceProfiler : IPluginExecutorObserver
{
    void ExportProfilerLogs(ILogExporter exporter);
}