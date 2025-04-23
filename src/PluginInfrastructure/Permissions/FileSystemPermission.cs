namespace PluginAPI.Models.Permissions;

public class FileSystemPermission(string path, bool canRead = true, bool canWrite = true, bool recursive = true) 
    : PermissionBase(path)
{
    public bool CanRead { get; } = canRead;
    public bool CanWrite { get; } = canWrite;
    public bool Recursive { get; } = recursive;
}