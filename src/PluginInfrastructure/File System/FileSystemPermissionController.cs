using PluginAPI.Services.interfaces;

namespace PluginAPI.Services;

public class FileSystemPermissionController : IFileSystemPermissionController
{
    private readonly HashSet<string> _allowedDirectories = new();
    
    public void AddAllowedDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' does not exist");
        
        _allowedDirectories.Add(directoryPath);
    }

    public IReadOnlyCollection<string> GetAllowedDirectories()
        => _allowedDirectories;
}