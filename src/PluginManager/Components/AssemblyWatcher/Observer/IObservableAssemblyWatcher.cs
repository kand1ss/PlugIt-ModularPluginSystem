namespace ModularPluginAPI.Components.AssemblyWatcher.Observer;

public interface IObservableAssemblyWatcher
{
    void AddObserver(IAssemblyWatcherObserver observer);
    void RemoveObserver(IAssemblyWatcherObserver observer);
}