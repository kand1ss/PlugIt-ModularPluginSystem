using PluginAPI.Models.Permissions;

namespace PluginAPI.Services.interfaces;

/// <summary>
/// Defines methods for controlling and managing permissions related to file system access,
/// particularly for specifying and retrieving directories deemed safe for operation.
/// </summary>
public interface IFileSystemPermissionController
{
    /// Adds a directory to the list of allowed directories with specific file system permissions.
    /// <param name="path">The path of the directory to be added. The path is normalized before being stored.</param>
    /// <param name="permission">The file system permission associated with the directory, specifying read/write access. If null, default permissions allowing both reading and writing are applied.</param>
    void AddAllowedDirectory(string path, FileSystemPermission permission);

    /// <summary>
    /// Retrieves the collection of directories that are allowed for file system operations.
    /// </summary>
    /// <returns>An IReadOnlyCollection of strings representing the allowed directory paths.</returns>
    IReadOnlyDictionary<string, FileSystemPermission> GetAllowedDirectories();
}