using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public static class PluginMetadataGenerator
{
    public static PluginMetadata Generate(IPlugin plugin)
    {
        return new()
        {
            Name = plugin.Name,
            Version = plugin.Version,
        };
    }
    
    public static IEnumerable<PluginMetadata> Generate(IEnumerable<IPlugin> plugins)
        => plugins.Select(Generate);
}