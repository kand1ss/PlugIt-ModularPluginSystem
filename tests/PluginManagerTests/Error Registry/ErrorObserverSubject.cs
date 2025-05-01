using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Interfaces;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;
using PluginAPI;

namespace PluginManagerTests;

public class ErrorObserverSubject : IObservableErrorHandledPluginExecutor
{
    private readonly List<IErrorHandledPluginExecutorObserver> _observers = new();
    
    public void AddObserver(IErrorHandledPluginExecutorObserver observer)
        => _observers.Add(observer);

    public void RemoveObserver(IErrorHandledPluginExecutorObserver observer)
        => _observers.Remove(observer);


    public void AddError(IPluginData plugin)
        => AddError(plugin, new Exception());

    public void AddError(IPluginData plugin, Exception exception)
    {
        var metadata = PluginMetadataGenerator.Generate(plugin);
        var pluginInfo = PluginStatusMapper.Map(metadata);
        
        foreach(var observer in _observers)
            observer.OnPluginFaulted(pluginInfo, exception);
    }
}