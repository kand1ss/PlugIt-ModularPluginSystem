using PluginAPI.Models.Permissions;
using PluginAPI.Services.interfaces;
using PluginInfrastructure;

namespace PluginAPI.Services;

public class FileSystemPermissionController : IFileSystemPermissionController
{
    private readonly Dictionary<string, FileSystemPermission> _allowedDirectories = new();
    
    public void AddAllowedDirectory(string path, FileSystemPermission? permission = null)
    {
        permission ??= new FileSystemPermission();
        _allowedDirectories.Add(Normalizer.NormalizeDirectoryPath(path), permission);
    }

    public IReadOnlyDictionary<string, FileSystemPermission> GetAllowedDirectories()
        => _allowedDirectories;
}