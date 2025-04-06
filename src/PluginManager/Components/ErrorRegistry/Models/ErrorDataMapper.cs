using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.ErrorRegistry.Models;

public static class ErrorDataMapper
{
    public static ErrorData Map(PluginInfo pluginInfo, Exception exception)
        => new()
        {
            PluginName = pluginInfo.Name,
            PluginVersion = pluginInfo.Version,
            StateBeforeError = pluginInfo.State,
            ExceptionType = exception.GetType(),
            ErrorMessage = exception.Message,
            ErrorStackTrace = exception.StackTrace ?? "",
        };
}