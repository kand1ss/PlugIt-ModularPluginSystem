using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.ErrorRegistry.Models;

public class ErrorData
{
    public string PluginName { get; init; } = "";
    public string PluginVersion { get; init; } = "";
    public PluginState StateBeforeError { get; init; }
    public Type? ExceptionType { get; init; }
    public string ErrorMessage { get; init; } = "";
    public string ErrorStackTrace { get; init; } = "";
    public DateTime Timestamp { get; } = DateTime.Now;
}