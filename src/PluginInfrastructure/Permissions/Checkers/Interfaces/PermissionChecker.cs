using PluginAPI.Models.Permissions;

namespace PluginInfrastructure.Permissions.Checkers.Interfaces;

public abstract class PermissionChecker<T> where T : PermissionBase
{
    public abstract bool CheckPermissionExists(string path, out T? permission);

    public bool CheckPermissionsExists(IList<string> paths)
    {
        foreach(var path in paths)
        {
            if (!CheckPermissionExists(path, out var _))
                return false;
        }
        
        return true;
    }
}