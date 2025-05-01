using ModularPluginAPI.Components.Lifecycle;

namespace ModularPluginAPI.Components.Observer;

public interface IErrorHandledPluginExecutorObserver
{
    void OnPluginFaulted(PluginStatus plugin, Exception exception);
}