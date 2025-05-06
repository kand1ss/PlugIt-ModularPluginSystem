namespace ModularPluginAPI.Components.Observer;

public interface IObservableLoaderService
{
    void AddObserver(ILoaderServiceObserver observer);
    void RemoveObserver(ILoaderServiceObserver observer);
}