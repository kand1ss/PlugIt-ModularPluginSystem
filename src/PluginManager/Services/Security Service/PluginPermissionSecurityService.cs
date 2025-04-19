using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using PluginAPI.Models.Permissions;
using PluginInfrastructure;

namespace ModularPluginAPI.Components;

public class PluginPermissionSecurityService(PluginLoggingFacade logger) : IPluginPermissionSecurityService
{
    private readonly Dictionary<string, FileSystemPermission> _fileSystemPermissions = new();
    private readonly Dictionary<string, NetworkPermission> _networkPermissions = new();
    
    public void AddFileSystemPermission(string normalizedPath, bool canRead = true, bool canWrite = true, bool recursive = false)
    {
        AddFileSystemPermission(normalizedPath, canRead, canWrite);
        if (!recursive)
            return;

        var allSubdirectories = Directory.GetDirectories(normalizedPath, "*", SearchOption.AllDirectories);
        foreach(var path in allSubdirectories)
            AddFileSystemPermission(path, canRead, canWrite);
    }

    private void AddFileSystemPermission(string fullPath, bool canRead, bool canWrite)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(fullPath);
        if (_fileSystemPermissions.ContainsKey(normalizedPath))
            return;

        _fileSystemPermissions.Add(normalizedPath, new FileSystemPermission(canRead, canWrite));
        logger.FilePermissionAdded(fullPath, canRead, canWrite);
    }

    public void AddNetworkPermission(string url, bool canGet = true, bool canPost = true)
    {
        var normalizedUrl = Normalizer.NormalizeUrl(url);
        if (_networkPermissions.ContainsKey(normalizedUrl))
            return;

        _networkPermissions.Add(normalizedUrl, new NetworkPermission(canGet, canPost));
        logger.NetworkPermissionAdded(url, canGet, canPost);
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