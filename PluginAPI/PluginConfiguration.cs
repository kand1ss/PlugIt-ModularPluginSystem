using System.Text.Json.Serialization;

namespace PluginAPI;

public class PluginConfiguration
{
    [JsonInclude]
    public Dictionary<string, Version> Dependencies { get; private set; } = new();
}