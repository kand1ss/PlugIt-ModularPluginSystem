using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components;

public interface ILoggerService
{
    void Log(LogSender sender, LogType logType, string message);
    IEnumerable<string> GetLogs();
    void ExportLogs(ILogExporter exporter);
    void ExportLogs(ILogExporter exporter, IEnumerable<LogType> exceptLogTypes);
}