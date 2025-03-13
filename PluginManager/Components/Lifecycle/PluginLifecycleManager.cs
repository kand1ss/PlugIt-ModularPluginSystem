
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
    
    
    public void UnloadPlugin(string pluginName)
        => _pluginStates.Remove(pluginName);
    public void UnloadPlugins(IEnumerable<string> pluginNames)
        => pluginNames.ToList().ForEach(UnloadPlugin);
    public void SetAllPluginsUnloaded()
        => _pluginStates.ToList()
            .ForEach(p => SetPluginState(p.Key, PluginState.Unloaded));
    
    public void Clear() => _pluginStates.Clear();


    public IEnumerable<PluginInfo> GetLifecycleStatistics()
        => PluginInfoMapper.Map(_pluginStates);
    
    public PluginState GetPluginState(string plugin)
        => _pluginStates[plugin];
}