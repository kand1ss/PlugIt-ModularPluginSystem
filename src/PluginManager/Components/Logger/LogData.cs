namespace ModularPluginAPI.Components.Logger;

public class LogData
{
    private DateTime Created { get; } = DateTime.Now;
    public LogSender Sender { get; private init; }
    public LogType LogType { get; private init; }
    public string Message { get; private init; } = string.Empty;

    public static LogData Create(LogSender sender, LogType logType, string message)
        => new()
        {
            LogType = logType,
            Sender = sender,
            Message = message
        };

    public override string ToString()
        => $"({Created}) > [{Sender}] [{LogType}] {Message}";
}