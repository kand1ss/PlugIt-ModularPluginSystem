using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public static class PluginMetadataGenerator
{
    public static PluginMetadata Generate(IPlugin plugin)
    {
        PluginConfiguration? configuration = null;
        if (plugin is IConfigurablePlugin configurable)
            configuration = configurable.Configuration;
        
        var metadata = new PluginMetadata
        {
            Name = plugin.Name,
            Version = plugin.Version,
            Author = plugin.Author,
            Description = plugin.Description,
            Configuration = configuration
        };
        
        return metadata;
    }
    
    public static IEnumerable<PluginMetadata> Generate(IEnumerable<IPlugin> plugins)
        => plugins.Select(Generate);
}