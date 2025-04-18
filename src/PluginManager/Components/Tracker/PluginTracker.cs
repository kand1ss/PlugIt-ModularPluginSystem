using ModularPluginAPI.Components.Lifecycle.Observer;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle;

public class PluginTracker(PluginLoggingFacade logger) : IPluginTracker, 
    IErrorHandledPluginExecutorObserver, IMetadataRepositoryObserver, IPluginExecutorObserver
{
    private readonly List<IPluginTrackerObserver> _observers = new();
    private readonly Dictionary<string, PluginInfo> _plugins = new();
    
    private void NotifyObservers(Action<IPluginTrackerObserver> action)
    {
        foreach(var observer in _observers)
            action(observer);
    }

    public void AddObserver(IPluginTrackerObserver observer)
    {
        _observers.Add(observer);
        logger.TrackerComponentRegistered(observer);
    }

    public void RemoveObserver(IPluginTrackerObserver observer)
    {
        _observers.Remove(observer);
        logger.TrackerComponentRemoved(observer);
    }

    public void OnMetadataAdded(AssemblyMetadata assemblyMetadata)
    {
        var plugins = assemblyMetadata.Plugins;
        RegisterPlugins(plugins);
    }

    public void OnMetadataRemoved(AssemblyMetadata assemblyMetadata)
    {
        var pluginNames = assemblyMetadata.Plugins.Select(x => x.Name);
        RemovePlugins(pluginNames);
    }

    public void OnPluginStateChanged(PluginInfo plugin)
        => SetPluginState(plugin.Name, plugin.State);
    public void OnPluginFaulted(PluginInfo plugin, Exception exception)
        => SetPluginState(plugin.Name, PluginState.Faulted);

    
    
    public void RegisterPlugin(PluginMetadata plugin)
    {
        var pluginInfo = PluginInfoMapper.Map(plugin);
        if(_plugins.TryAdd(plugin.Name, pluginInfo))
            NotifyObservers(o => o.OnPluginRegistered(pluginInfo));
    }

    public void RegisterPlugins(IEnumerable<PluginMetadata> plugins)
    {
        foreach(var plugin in plugins)
            RegisterPlugin(plugin);
    }


    public void RemovePlugin(string pluginName)
    {
        var pluginInfo = _plugins[pluginName];
        if(_plugins.Remove(pluginName))
            NotifyObservers(o => o.OnPluginRemoved(pluginInfo));
    }

    public void RemovePlugins(IEnumerable<string> pluginNames)
    {
        foreach(var pluginName in pluginNames)
            RemovePlugin(pluginName);
    }

    public void Clear() => _plugins.Clear();


    public void SetPluginState(string pluginName, PluginState state)
    {
        if (_plugins.TryGetValue(pluginName, out var info) && info.State != state)
        {
            info.State = state;
            NotifyObservers(o => o.OnPluginStateChanged(info));
            logger.PluginStateChanged(pluginName, state);
        }
    }

    public void SetPluginsState(IEnumerable<string> pluginNames, PluginState state)
        => pluginNames.ToList().ForEach(n => SetPluginState(n, state));


    public IEnumerable<PluginInfo> GetPluginsStatus()
        => _plugins.Values;

    public PluginInfo GetPluginStatus(string plugin)
        => _plugins[plugin];
}