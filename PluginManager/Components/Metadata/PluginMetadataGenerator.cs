using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public static class PluginMetadataGenerator
{
    public static PluginMetadata Generate(IPlugin plugin)
    {
        var metadata = new PluginMetadata
        {
            Name = plugin.Name,
            Version = plugin.Version,
            Author = plugin.Author,
            Description = plugin.Description
        };

        if (plugin is IConfigurablePlugin configurable)
            metadata.Dependencies = configurable.Configuration?.Dependencies ?? [];
        
        return metadata;
    }
    
    public static IEnumerable<PluginMetadata> Generate(IEnumerable<IPlugin> plugins)
        => plugins.Select(Generate);
}