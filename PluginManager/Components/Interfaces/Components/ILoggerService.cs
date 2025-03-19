using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public interface ILoggerService
{
    void Log(LogSender sender, LogType logType, string message);
    IEnumerable<string> GetLogMessages();
    void WriteMessagesToFile(string path);
}