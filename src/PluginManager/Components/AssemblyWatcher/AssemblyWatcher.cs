using ModularPluginAPI.Components.AssemblyWatcher.Interfaces;
using ModularPluginAPI.Components.AssemblyWatcher.Observer;

namespace ModularPluginAPI.Components.AssemblyWatcher;

public class AssemblyWatcher : IAssemblyWatcher
{
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly List<IAssemblyWatcherObserver> _observers = new();

    private void NotifyObservers(Action<IAssemblyWatcherObserver> action)
    {
        foreach(var observer in _observers)
            action(observer);
    }
    
    public void AddObserver(IAssemblyWatcherObserver observer)
        => _observers.Add(observer);

    public void RemoveObserver(IAssemblyWatcherObserver observer)
        => _observers.Remove(observer);
    
    
    public void ObserveAssembly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            return;
        
        var watcher = new FileSystemWatcher(Path.GetDirectoryName(assemblyPath)!, Path.GetFileName(assemblyPath));
        watcher.Created += OnFileAdded;
        watcher.Changed += OnFileChanged;
        watcher.Deleted += OnFileRemoved;
        watcher.EnableRaisingEvents = true;
        
        _watchers.Add(assemblyPath, watcher);
    }
    

    private void OnFileAdded(object sender, FileSystemEventArgs e)
        => NotifyObservers(a => a.OnAssemblyAdded(e.FullPath));
    
    private void OnFileRemoved(object sender, FileSystemEventArgs e)
        => NotifyObservers(a => a.OnAssemblyRemoved(e.FullPath));
    
    private void OnFileChanged(object sender, FileSystemEventArgs e)
        => NotifyObservers(a => a.OnAssemblyChanged(e.FullPath));
    

    public void UnobserveAssembly(string assemblyPath)
    {
        if(!_watchers.TryGetValue(assemblyPath, out var watcher))
            return;
        
        watcher.Created -= OnFileAdded;
        watcher.Changed -= OnFileChanged;
        watcher.Deleted -= OnFileRemoved;
        watcher.EnableRaisingEvents = false;
        
        _watchers.Remove(assemblyPath);
    }
}