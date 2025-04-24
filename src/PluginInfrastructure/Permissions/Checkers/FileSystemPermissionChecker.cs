using System.Diagnostics.CodeAnalysis;
using PluginAPI.Models.Permissions;
using PluginInfrastructure.Permissions;
using PluginInfrastructure.Permissions.Checkers.Interfaces;

namespace PluginAPI.Services;

public class FileSystemPermissionChecker(IReadOnlyDictionary<string, FileSystemPermission> permissions) 
    : PermissionChecker<FileSystemPermission>
{
    private readonly FileSystemPermissionSeeker _permissionSeeker = new(permissions);
    
    public override bool CheckPermissionExists(string path, [NotNullWhen(true)] out FileSystemPermission? permission)
    {
        permission = _permissionSeeker.GetSuitablePermission(path);
        return permission != null;
    }
}