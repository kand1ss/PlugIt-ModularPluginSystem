namespace PluginAPI.Models;

public class PluginPermissions
{
    public HashSet<string> FileSystemPaths { get; private set; } = new();
    public HashSet<string> NetworkURLs { get; private set; } = new();
}