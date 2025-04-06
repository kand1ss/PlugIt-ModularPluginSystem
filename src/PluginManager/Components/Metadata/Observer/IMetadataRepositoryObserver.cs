using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Observer;

public interface IMetadataRepositoryObserver
{
    void OnMetadataAdded(AssemblyMetadata assemblyMetadata);
    void OnMetadataRemoved(AssemblyMetadata assemblyMetadata);
    void OnMetadataCleared();
}