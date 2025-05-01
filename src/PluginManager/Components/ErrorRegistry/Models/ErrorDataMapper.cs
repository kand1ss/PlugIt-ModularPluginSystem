using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.ErrorRegistry.Models;

public static class ErrorDataMapper
{
    public static ErrorData Map(PluginStatus pluginStatus, Exception exception)
        => new()
        {
            PluginName = pluginStatus.Name,
            PluginVersion = pluginStatus.Version.ToString(),
            StateBeforeError = pluginStatus.CurrentState,
            ModeBeforeError = pluginStatus.CurrentMode,
            ExceptionType = exception.GetType(),
            ErrorMessage = exception.Message,
            ErrorStackTrace = exception.StackTrace ?? ""
        };
}