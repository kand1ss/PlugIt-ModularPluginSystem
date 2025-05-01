using PluginAPI;

namespace ModularPluginAPI.Models;

public class PluginMetadata
{
    public string Name { get; init; } = string.Empty;
    public Version Version { get; init; } = new(0, 0, 0);
    public string Author { get; init; } = string.Empty;

    public PluginConfiguration Configuration { get; init; } = new();
}