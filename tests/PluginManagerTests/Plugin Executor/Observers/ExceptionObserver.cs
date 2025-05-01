using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;

namespace PluginManagerTests;

public class ExceptionObserver : IErrorHandledPluginExecutorObserver
{
    public List<string> AddedErrors = new();
    
    public void OnPluginFaulted(PluginStatus plugin, Exception exception)
        => AddedErrors.Add(plugin.Name);
}