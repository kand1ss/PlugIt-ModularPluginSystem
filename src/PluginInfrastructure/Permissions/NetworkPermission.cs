namespace PluginAPI.Models.Permissions;

public class NetworkPermission(string path, bool canGet = true, bool canPost = true) : PermissionBase(path)
{
    public bool CanGet { get; } = canGet;
    public bool CanPost { get; } = canPost;
}