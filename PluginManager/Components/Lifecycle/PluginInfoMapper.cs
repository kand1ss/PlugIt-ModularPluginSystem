namespace ModularPluginAPI.Components.Lifecycle;

public static class PluginInfoMapper
{
    public static PluginInfo Map(string plugin, PluginState state)
        => new()
        {
            PluginName = plugin,
            PluginState = state.ToString(),
        };
    
    public static IEnumerable<PluginInfo> Map(Dictionary<string, PluginState> pluginStates)
        => pluginStates.Select(t => Map(t.Key, t.Value));
}