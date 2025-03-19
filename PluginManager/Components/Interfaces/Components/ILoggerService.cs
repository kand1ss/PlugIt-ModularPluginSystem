using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface ILoggerService
{
    void Log(LogSender sender, LogType logType, string message);
    IEnumerable<string> GetLogMessages();
    void WriteMessagesToFile(string path);
}