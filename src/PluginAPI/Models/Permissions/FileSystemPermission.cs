namespace PluginAPI.Models.Permissions;

public class FileSystemPermission(bool canRead = true, bool canWrite = true, bool recursive = true)
{
    public bool CanRead { get; } = canRead;
    public bool CanWrite { get; } = canWrite;
    public bool Recursive { get; } = recursive;
}