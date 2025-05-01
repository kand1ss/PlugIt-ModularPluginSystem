using ModularPluginAPI.Models;
using PluginAPI;

namespace ModularPluginAPI.Components;

public static class PluginMetadataGenerator
{
    public static PluginMetadata Generate(IPluginData plugin)
    {
        PluginConfiguration? configuration = null;
        if (plugin is IConfigurablePlugin configurable)
            configuration = configurable.Configuration;
        
        var metadata = new PluginMetadata
        {
            Name = plugin.Name,
            Version = plugin.Version,
            Author = plugin.Author,
            Configuration = configuration ?? new PluginConfiguration(),
        };
        
        return metadata;
    }
    
    public static IEnumerable<PluginMetadata> Generate(IEnumerable<IPluginData> plugins)
        => plugins.Select(Generate);
}