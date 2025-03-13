namespace ModularPluginAPI.Components.Lifecycle;

public static class PluginInfoMapper
{
    public static IEnumerable<PluginInfo> Map(Dictionary<string, PluginState> pluginStates)
    {
        var pluginInfos = new List<PluginInfo>();
        foreach (var plugin in pluginStates)
            pluginInfos.Add(new PluginInfo()
            {
                PluginName = plugin.Key,
                PluginState = plugin.Value.ToString(),
            });
        return pluginInfos;
    }
}