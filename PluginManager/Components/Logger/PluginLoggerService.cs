using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Logger;

public class PluginLoggerService(ILogRepository logRepository) : ILoggerService
{
    public void Log(LogSender sender, LogType logType, string message)
        => logRepository.Add(sender, logType, message);

    public IEnumerable<string> GetLogs()
        => logRepository.GetLogs();

    private void ExportMessages(ILogExporter exporter, IEnumerable<string> messages)
    {
        exporter.Export(messages);
        logRepository.ClearLogs();
    }
    public void ExportLogs(ILogExporter exporter)
    {
        var messages = logRepository.GetLogs();
        ExportMessages(exporter, messages);
    }
    public void ExportLogs(ILogExporter exporter, IEnumerable<LogType> exceptLogTypes)
    {
        var messages = logRepository.GetLogsExceptByLogTypes(exceptLogTypes);
        ExportMessages(exporter, messages);
    }
}