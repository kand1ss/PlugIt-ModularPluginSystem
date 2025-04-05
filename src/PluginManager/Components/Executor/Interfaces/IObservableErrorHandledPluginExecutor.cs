using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components.Interfaces;

public interface IObservableErrorHandledPluginExecutor
{
    void AddObserver(IErrorHandledPluginExecutorObserver observer);
    void RemoveObserver(IErrorHandledPluginExecutorObserver observer);
}