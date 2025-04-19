using PluginAPI.Services.interfaces;
using PluginInfrastructure;

namespace PluginAPI.Services;

public class FileSystemPermissionController : IFileSystemPermissionController
{
    private readonly List<string> _allowedDirectories = new();
    
    public void AddAllowedDirectory(string path)
        => _allowedDirectories.Add(Normalizer.NormalizeDirectoryPath(path));

    public IReadOnlyCollection<string> GetAllowedDirectories()
        => _allowedDirectories;
}