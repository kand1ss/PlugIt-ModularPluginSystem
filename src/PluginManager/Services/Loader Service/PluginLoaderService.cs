using System.Reflection;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Exceptions;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginLoaderService(PluginMetadataService metadataService, IAssemblyLoader loader, 
    IAssemblyHandler handler) : IPluginLoaderService, IObservableLoaderService
{
    private readonly List<ILoaderServiceObserver> _observers = new();

    private void NotifyObservers(Action<ILoaderServiceObserver> action)
    {
        foreach (var observer in _observers)
            action(observer);
    }
    public void AddObserver(ILoaderServiceObserver observer)
        => _observers.Add(observer);
    public void RemoveObserver(ILoaderServiceObserver observer)
        => _observers.Remove(observer);
    
    
    public Assembly LoadAssembly(string assemblyName)
    {
        var assembly = loader.LoadAssembly(assemblyName)
               ?? throw new AssemblyNotFoundException(assemblyName);
        
        NotifyObservers(o => o.OnAssemblyLoaded(
            metadataService.GetMetadata(assemblyName)));;

        return assembly;
    }

    public Assembly LoadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        return LoadAssembly(metadata.Path);
    }

    public void UnloadAssembly(string assemblyName)
    {
        loader.UnloadAssembly(assemblyName);
        NotifyObservers(o => o.OnAssemblyUnloaded(
            metadataService.GetMetadata(assemblyName)));
    }

    public void UnloadAssemblyByPluginName(string pluginName)
    {
        var metadata = metadataService.GetMetadataByPluginName(pluginName);
        UnloadAssembly(metadata.Path);
    }

    public T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPluginData
        => handler.GetPlugin<T>(assembly, pluginName)
           ?? throw new PluginNotFoundException(pluginName);

}