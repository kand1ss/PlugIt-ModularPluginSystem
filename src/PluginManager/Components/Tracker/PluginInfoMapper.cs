using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle;

public static class PluginInfoMapper
{
    public static PluginInfo Map(PluginMetadata pluginMetadata)
        => new()
        {
            Name = pluginMetadata.Name,
            Version = pluginMetadata.Version.ToString(),
            Author = pluginMetadata.Author,
            State = PluginState.Unloaded
        };
    
    public static IEnumerable<PluginInfo> Map(IEnumerable<PluginMetadata> pluginMetadata)
        => pluginMetadata.Select(Map);
}