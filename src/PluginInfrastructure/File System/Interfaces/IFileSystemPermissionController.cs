namespace PluginAPI.Services.interfaces;

public interface IFileSystemPermissionController
{
    void AddAllowedDirectory(string directoryPath);
    IReadOnlyCollection<string> GetAllowedDirectories();
}