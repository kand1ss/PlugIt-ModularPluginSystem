using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Logger.Components;

public class LogRepository : ILogRepository
{
    private readonly HashSet<LogData> _logs = new();
    
    private IEnumerable<string> ParseLogsToString(IEnumerable<LogData> logs)
        => logs.Select(m => m.ToString());
    private IEnumerable<string> ParseLogsToString()
        => ParseLogsToString(_logs);

    public void Add(LogSender sender, LogType logType, string message)
        => _logs.Add(LogData.Create(sender, logType, message));

    public IEnumerable<string> GetLogs()
        => ParseLogsToString();
    public void ClearLogs()
        => _logs.Clear();
    private IEnumerable<LogData> GetLogsExceptLogType(IEnumerable<LogType> exceptLogTypes)
        => _logs.Where(m => !exceptLogTypes.Contains(m.LogType));

    public IEnumerable<string> GetLogsExceptByLogTypes(IEnumerable<LogType> exceptLogTypes)
    {
        var logs = GetLogsExceptLogType(exceptLogTypes);
        return ParseLogsToString(logs);
    }
}