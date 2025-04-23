namespace PluginAPI.Models.Permissions;

public class PermissionBase(string path)
{
    public string Path { get; set; } = path;
}