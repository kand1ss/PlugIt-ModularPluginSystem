using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components.Interfaces;

public interface IObservableMetadataRepository
{
    void AddObserver(IMetadataRepositoryObserver observer);
    void RemoveObserver(IMetadataRepositoryObserver observer);
}