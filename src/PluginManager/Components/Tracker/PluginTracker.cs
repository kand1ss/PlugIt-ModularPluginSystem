using ModularPluginAPI.Components.Lifecycle.Observer;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle;

public class PluginTracker(PluginLoggingFacade logger) : IPluginTracker
{
    private readonly List<IPluginTrackerObserver> _observers = new();
    private readonly Dictionary<string, PluginStatus> _plugins = new();
    
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
    
    public void RegisterPlugin(PluginMetadata plugin)
    {
        var pluginInfo = PluginStatusMapper.Map(plugin);
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


    public void SetPluginStatus(string pluginName, PluginState state, PluginMode mode)
    {
        if (_plugins.TryGetValue(pluginName, out var info) && info.CurrentState != state)
        {
            info.CurrentState = state;
            info.CurrentMode = mode;
            NotifyObservers(o => o.OnPluginStatusChanged(info));
            logger.PluginStateChanged(pluginName, state);
        }
    }

    public void SetPluginsStatus(IEnumerable<string> pluginNames, PluginState state, PluginMode mode)
        => pluginNames.ToList().ForEach(n => SetPluginStatus(n, state, mode));


    public IEnumerable<PluginStatus> GetPluginsStatus()
        => _plugins.Values;

    public PluginStatus GetPluginStatus(string plugin)
        => _plugins[plugin];
}