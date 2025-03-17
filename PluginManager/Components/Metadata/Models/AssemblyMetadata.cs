using ModularPluginAPI.Context;

namespace ModularPluginAPI.Models;

public class AssemblyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Version Version { get; set; } = new(0, 0, 0);
    public string Author { get; set; } = string.Empty;
    public HashSet<PluginMetadata> Plugins { get; set; } = new();
}