using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Lifecycle;

public static class PluginStatusMapper
{
    public static PluginStatus Map(PluginMetadata pluginMetadata, PluginState state = PluginState.Unloaded, PluginMode mode = PluginMode.Idle)
        => new()
        {
            Name = pluginMetadata.Name,
            Version = pluginMetadata.Version,
            Author = pluginMetadata.Author,
            CurrentState = state,
            CurrentMode = mode
        };
    
    public static IEnumerable<PluginStatus> Map(IEnumerable<PluginMetadata> pluginMetadata, PluginState state = PluginState.Unloaded, PluginMode mode = PluginMode.Fixed)
        => pluginMetadata.Select(plugin => Map(plugin, state, mode));
}