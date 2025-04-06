using ModularPluginAPI.Components.ErrorRegistry.Models;

namespace ModularPluginAPI.Components.ErrorRegistry.Observer;

public interface IPluginErrorRegistryObserver
{
    void OnErrorAdded(ErrorData errorData);
    void OnErrorRemoved(ErrorData errorData);
}