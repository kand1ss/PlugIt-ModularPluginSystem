using ModularPluginAPI.Components.Lifecycle;
using PluginAPI;

namespace ModularPluginAPI.Components.Logger;

public class PluginLoggerService : ILoggerService
{
    private readonly List<string> _messages = new();
    
    private string CreateMessage(LogSender sender, LogType logType, string message)
        => $"({DateTime.Now}) > [{sender}] [{logType}] {message}";
    
    
    public void Log(LogSender sender, LogType logType, string message)
        => _messages.Add(CreateMessage(sender, logType, message));
    
    public void LogState(string pluginName, PluginState state)
        => Log(LogSender.Plugin, LogType.INFO, 
            $"Plugin state changed | Plugin: {pluginName} | State: {state}");
    
    
    public IEnumerable<string> GetLogMessages()
        => _messages;

    public void WriteMessagesToFile(string path)
    {
        var date = DateTime.Now;
        var fileName = $"Log_{date:yyyy-MM-dd_HH-mm-ss}.log";
        var fullPath = Path.Combine(path, fileName);
        
        File.AppendAllLines(fullPath, _messages);
    }
}