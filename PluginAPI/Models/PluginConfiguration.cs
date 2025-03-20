using System.Text.Json.Serialization;
using PluginAPI.Dependency;

namespace PluginAPI;

public class PluginConfiguration
{
    [JsonInclude]
    public HashSet<DependencyInfo> Dependencies { get; private set; } = new();
}