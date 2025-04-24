using PluginAPI.Models.Permissions;

namespace PluginInfrastructure.Permissions;

public class FileSystemPermissionSeeker(IReadOnlyDictionary<string, FileSystemPermission> permissions) 
{
    public FileSystemPermission? GetSuitablePermission(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        
        path = Normalizer.NormalizeDirectoryPath(path);
        if (permissions.TryGetValue(path, out var permission))
            return permission;

        if (Path.HasExtension(path))
        {
            var directory = Path.GetDirectoryName(path) ?? "";
            return GetSuitablePermission(directory);
        }

        foreach (var kvp in permissions)
        {
            if (!kvp.Value.Recursive)
                continue;
            
            if (path.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        return null;
    }
}