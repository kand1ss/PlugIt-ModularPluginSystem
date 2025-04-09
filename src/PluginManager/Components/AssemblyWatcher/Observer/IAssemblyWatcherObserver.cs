namespace ModularPluginAPI.Components.AssemblyWatcher.Observer;

public interface IAssemblyWatcherObserver
{
    void OnAssemblyAdded(string assemblyPath);
    void OnAssemblyRemoved(string assemblyPath);
    void OnAssemblyChanged(string assemblyPath);
}