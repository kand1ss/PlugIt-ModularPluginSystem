using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components;

public interface IPluginPerformanceProfiler
{
    void ExportProfilerLogs(ILogExporter exporter);
}