using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Logger;

public class PluginLoggerService(ILogRepository logRepository) : ILoggerService
{
    public void Log(LogSender sender, LogType logType, string message)
        => logRepository.Add(sender, logType, message);

    public IEnumerable<string> GetLogs()
        => logRepository.GetLogs();

    public void ExportLogs(ILogExporter exporter)
    {
        var messages = logRepository.GetLogs();
        exporter.Export(messages);
    }
    public void ExportLogs(ILogExporter exporter, IEnumerable<LogType> exceptLogTypes)
    {
        var messages = logRepository.GetLogsExceptByLogTypes(exceptLogTypes);
        exporter.Export(messages);
    }
}