using PluginAPI.Models.Permissions;

namespace PluginInfrastructure.Permissions.Checkers.Interfaces;

public abstract class PermissionChecker<T> where T : PermissionBase
{
    public abstract bool CheckPermissionAllow(string path, out T? permission);

    public bool CheckPermissionsAllow(IList<string> paths)
    {
        foreach(var path in paths)
        {
            if (!CheckPermissionAllow(path, out var _))
                return false;
        }
        
        return true;
    }
}