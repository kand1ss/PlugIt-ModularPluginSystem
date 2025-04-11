using ModularPluginAPI.Components.Interfaces;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;

namespace PluginManagerTests.Plugin_Tracker;

public class TrackerObserverSubject : IObservableMetadataRepository
{
    private readonly List<IMetadataRepositoryObserver> _observers = new();
    
    public void AddObserver(IMetadataRepositoryObserver observer)
        => _observers.Add(observer);
    public void RemoveObserver(IMetadataRepositoryObserver observer)
        => _observers.Remove(observer);


    public void AddMetadata(AssemblyMetadata assemblyMetadata)
    {
        foreach(var observer in _observers)
            observer.OnMetadataAdded(assemblyMetadata);
    }

    public void RemoveMetadata(AssemblyMetadata assemblyMetadata)
    {
        foreach(var observer in _observers)
            observer.OnMetadataRemoved(assemblyMetadata);
    }
}