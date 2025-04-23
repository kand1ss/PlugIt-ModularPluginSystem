using PluginAPI.Models.Permissions;
using PluginInfrastructure;
using PluginInfrastructure.Permissions.Checkers.Interfaces;

namespace PluginAPI.Services;

public class FileSystemPermissionChecker(IReadOnlyDictionary<string, FileSystemPermission> permissions) 
    : PermissionChecker<FileSystemPermission>
{
    public override bool CheckPermissionAllow(string path, out FileSystemPermission? permission)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(path);
        if (permissions.TryGetValue(normalizedPath, out permission))
            return true;

        if (Path.HasExtension(path))
        {
            var parentPath = Normalizer.NormalizeDirectoryPath(Path.GetDirectoryName(path) ?? "");
            if (permissions.TryGetValue(parentPath, out permission))
                return true;
        }
        
        foreach (var kv in permissions)
        {
            if (!kv.Value.Recursive)
                continue;

            if (normalizedPath.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                permission = kv.Value;
                return true;
            }
        }
        
        permission = null;
        return false;
    }
}