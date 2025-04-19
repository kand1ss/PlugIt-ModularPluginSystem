using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using PluginAPI.Models.Permissions;
using PluginInfrastructure;

namespace ModularPluginAPI.Components;

public class PluginPermissionSecurityService : IPluginPermissionSecurityService
{
    private readonly Dictionary<string, FileSystemPermission> _fileSystemPermissions = new();
    private readonly Dictionary<string, NetworkPermission> _networkPermissions = new();
    
    public void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = false)
    {
        AddFileSystemPermission(fullPath, canRead, canWrite);
        if (!recursive)
            return;

        var allSubdirectories = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
        foreach(var path in allSubdirectories)
            AddFileSystemPermission(path, canRead, canWrite);
    }

    private void AddFileSystemPermission(string fullPath, bool canRead, bool canWrite)
    {
        fullPath = Normalizer.NormalizeDirectoryPath(fullPath);
        if (_fileSystemPermissions.ContainsKey(fullPath))
            return;

        _fileSystemPermissions.Add(fullPath, new FileSystemPermission(canRead, canWrite));
    }

    public void AddNetworkPermission(string url, bool canGet = true, bool canPost = true)
    {
        url = Normalizer.NormalizeUrl(url);
        if (_networkPermissions.ContainsKey(url))
            return;

        _networkPermissions.Add(url, new NetworkPermission(canGet, canPost));
    }
    
    public IDictionary<string, FileSystemPermission> GetFileSystemPermissions()
        => _fileSystemPermissions;

    public IDictionary<string, NetworkPermission> GetNetworkPermissions()
        => _networkPermissions;

    
    public bool CheckSafety(PluginMetadata pluginMetadata)
    {
        var permissions = pluginMetadata.Configuration.Permissions;
        return 
            CheckFileSystemPermissions(permissions.FileSystemPaths) && 
            CheckNetworkPermissions(permissions.NetworkURLs);
    }


    private bool CheckFileSystemPermissions(IEnumerable<string> filePaths)
        => filePaths.All(path =>
        {
            var normalizedPath = Normalizer.NormalizeDirectoryPath(path);
            return _fileSystemPermissions.Keys.Any(x 
                => normalizedPath.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        });

    private bool CheckNetworkPermissions(IEnumerable<string> networkPaths)
        => networkPaths.All(x => _networkPermissions.ContainsKey(Normalizer.NormalizeUrl(x)));
}