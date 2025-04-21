namespace PluginAPI.Services.interfaces;

/// <summary>
/// Defines methods for controlling and managing permissions related to file system access,
/// particularly for specifying and retrieving directories deemed safe for operation.
/// </summary>
public interface IFileSystemPermissionController
{
    /// Adds a directory to the list of allowed directories.
    /// <param name="path">The path of the directory to be added. The path is normalized before being stored.</param>
    void AddAllowedDirectory(string path);

    /// <summary>
    /// Retrieves the collection of directories that are allowed for file system operations.
    /// </summary>
    /// <returns>An IReadOnlyCollection of strings representing the allowed directory paths.</returns>
    IReadOnlyCollection<string> GetAllowedDirectories();
}