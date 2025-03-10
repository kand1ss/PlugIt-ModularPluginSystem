using ModularPluginAPI.Context;

namespace ModularPluginAPI.Models;

public class AssemblyMetadata
{
    public string Name { get; set; }
    public string Path { get; set; }
    public Version Version { get; set; }
    public string Author { get; set; }
    public HashSet<PluginMetadata> Plugins { get; set; }
}