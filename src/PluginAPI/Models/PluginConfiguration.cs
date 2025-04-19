using System.Text.Json.Serialization;
using PluginAPI.Dependency;
using PluginAPI.Models;

namespace PluginAPI;

public class PluginConfiguration
{
    [JsonInclude] public HashSet<DependencyInfo> Dependencies { get; init; } = new();
    [JsonInclude] public PluginPermissions Permissions { get; private set; } = new();
}