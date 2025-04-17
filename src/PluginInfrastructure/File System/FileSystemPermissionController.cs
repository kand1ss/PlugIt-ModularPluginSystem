using PluginAPI.Services.interfaces;
using PluginInfrastructure;

namespace PluginAPI.Services;

public class FileSystemPermissionController : IFileSystemPermissionController
{
    private readonly List<string> _allowedDirectories = new();
    
    public void AddAllowedDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' does not exist");

        _allowedDirectories.Add(Normalizer.NormalizeDirectoryPath(directoryPath));
    }

    public IReadOnlyCollection<string> GetAllowedDirectories()
        => _allowedDirectories;
}