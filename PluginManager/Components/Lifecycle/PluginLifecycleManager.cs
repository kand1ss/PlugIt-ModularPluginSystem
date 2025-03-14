
namespace ModularPluginAPI.Components.Lifecycle;

public class PluginLifecycleManager : IPluginLifecycleManager
{
    private readonly Dictionary<string, PluginState> _pluginStates = new();

    public void SetPluginState(string pluginName, PluginState state)
    {
        if (!_pluginStates.TryGetValue(pluginName, out _))
            _pluginStates.Add(pluginName, state);
        else
            _pluginStates[pluginName] = state;
    }
    public void SetPluginsState(IEnumerable<string> pluginNames, PluginState state)
        => pluginNames.ToList().ForEach(n => SetPluginState(n, state));
    
    
    public void RemovePlugin(string pluginName) => _pluginStates.Remove(pluginName);
    public void RemovePlugins(IEnumerable<string> pluginNames) 
        => pluginNames.ToList().ForEach(RemovePlugin);
    
    public void Clear() => _pluginStates.Clear();


    public IEnumerable<PluginInfo> GetPluginStates()
        => PluginInfoMapper.Map(_pluginStates);
    public PluginInfo GetPluginState(string plugin)
        => PluginInfoMapper.Map(plugin, _pluginStates[plugin]);
}