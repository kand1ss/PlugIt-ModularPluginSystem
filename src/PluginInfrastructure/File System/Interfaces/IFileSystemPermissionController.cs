namespace PluginAPI.Services.interfaces;

public interface IFileSystemPermissionController
{
    void AddAllowedDirectory(string path);
    IReadOnlyCollection<string> GetAllowedDirectories();
}