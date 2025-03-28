using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle;

public class PluginTracker(PluginLoggingFacade logger) : IPluginTracker
{
    private readonly Dictionary<string, PluginInfo> _plugins = new();

    public void RegisterPlugin(PluginMetadata plugin)
    {
        var pluginInfo = PluginInfoMapper.Map(plugin);
        _plugins.TryAdd(plugin.Name, pluginInfo);
    }
    public void RegisterPlugins(IEnumerable<PluginMetadata> plugins)
    {
        foreach(var plugin in plugins)
            RegisterPlugin(plugin);
    }
    
    
    public void RemovePlugin(string pluginName) => _plugins.Remove(pluginName);
    public void RemovePlugins(IEnumerable<string> pluginNames) 
        => pluginNames.ToList().ForEach(RemovePlugin);
    public void Clear() => _plugins.Clear();

    
    public void SetPluginState(string pluginName, PluginState state)
    {
        if (!_plugins.TryGetValue(pluginName, out _))
            return;
        if (_plugins[pluginName].State == state)
            return;
        
        _plugins[pluginName].State = state;
        logger.PluginStateChanged(pluginName, state);
    }
    public void SetPluginsState(IEnumerable<string> pluginNames, PluginState state)
        => pluginNames.ToList().ForEach(n => SetPluginState(n, state));


    public IEnumerable<PluginInfo> GetPluginsStatus()
        => _plugins.Values;
    public PluginInfo GetPluginStatus(string plugin)
        => _plugins[plugin];
}