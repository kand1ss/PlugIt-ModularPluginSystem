using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Observer;

public interface ILoaderServiceObserver
{
    void OnAssemblyLoaded(AssemblyMetadata assembly);
    void OnAssemblyUnloaded(AssemblyMetadata assembly);
}