using ModularPluginAPI.Components.AssemblyWatcher.Observer;

namespace PluginManagerTests;

public class WatcherObserver : IAssemblyWatcherObserver
{
    public List<string> AddedAssemblies = new();
    public List<string> RemovedAssemblies = new();
    
    public void OnAssemblyAdded(string assemblyPath)
        => AddedAssemblies.Add(assemblyPath);

    public void OnAssemblyRemoved(string assemblyPath)
        => RemovedAssemblies.Add(assemblyPath);

    public void OnAssemblyChanged(string assemblyPath)
        => AddedAssemblies.Add(assemblyPath);
}