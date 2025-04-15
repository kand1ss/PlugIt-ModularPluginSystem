using System.Diagnostics.CodeAnalysis;
using PluginAPI;

namespace ModularPluginAPI.Models;

public class PluginMetadata
{
    public string Name { get; init; } = string.Empty;
    public Version Version { get; init; } = new(0, 0, 0);
    public string Description { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    
    public PluginConfiguration? Configuration { get; init; }
    
    [MemberNotNullWhen(true, nameof(Configuration))]
    public bool HasConfiguration => Configuration is not null;
}