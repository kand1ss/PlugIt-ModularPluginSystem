namespace ModularPluginAPI.Components.ErrorRegistry.Observer;

public interface IObservablePluginErrorRegistry
{
    void AddObserver(IPluginErrorRegistryObserver observer);
    void RemoveObserver(IPluginErrorRegistryObserver observer);
}