using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components.Interfaces;

public interface IObservablePluginExecutor
{
    void AddObserver(IPluginExecutorObserver observer);
    void RemoveObserver(IPluginExecutorObserver observer);
}