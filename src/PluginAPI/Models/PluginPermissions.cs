using System.Text.Json.Serialization;

namespace PluginAPI.Models;

public class PluginPermissions
{
    [JsonInclude] public HashSet<string> FileSystemPaths { get; private set; } = new();
    [JsonInclude] public HashSet<string> NetworkURLs { get; private set; } = new();
}