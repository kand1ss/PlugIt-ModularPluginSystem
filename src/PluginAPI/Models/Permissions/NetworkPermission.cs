namespace PluginAPI.Models.Permissions;

public class NetworkPermission(bool canGet = true, bool canPost = true)
{
    public bool CanGet { get; } = canGet;
    public bool CanPost { get; } = canPost;
}