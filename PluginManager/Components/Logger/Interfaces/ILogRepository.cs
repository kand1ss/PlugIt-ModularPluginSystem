namespace ModularPluginAPI.Components.Logger.Interfaces;

public interface ILogRepository
{
    void Add(LogSender sender, LogType logType, string message);
    IEnumerable<string> GetLogs();
    void ClearLogs();
    IEnumerable<string> GetLogsExceptByLogTypes(IEnumerable<LogType> exceptLogTypes);
}