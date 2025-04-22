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
    
    public void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = true)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(fullPath);
        if (_fileSystemPermissions.ContainsKey(normalizedPath))
            return;

        _fileSystemPermissions.Add(normalizedPath, new FileSystemPermission(canRead, canWrite, recursive));
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
    
    public IReadOnlyDictionary<string, FileSystemPermission> GetFileSystemPermissions()
        => _fileSystemPermissions;

    public IReadOnlyDictionary<string, NetworkPermission> GetNetworkPermissions()
        => _networkPermissions;

    
    public bool CheckSafety(PluginMetadata pluginMetadata)
    {
        var permissions = pluginMetadata.Configuration.Permissions;
        return 
            CheckFileSystemPermissions(permissions.FileSystemPaths) && 
            CheckNetworkPermissions(permissions.NetworkURLs);
    }


    private bool CheckFileSystemPermissions(IEnumerable<string> filePaths)
    {
        foreach (var path in filePaths)
        {
            var normalizedPath = Normalizer.NormalizeDirectoryPath(path);
            if (_fileSystemPermissions.ContainsKey(normalizedPath))
                continue;

            return _fileSystemPermissions
                .Where(kv => kv.Value.Recursive)
                .Any(kv => normalizedPath.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));
        }

        return true;
    }

    private bool CheckNetworkPermissions(IEnumerable<string> networkPaths)
        => networkPaths.All(x => _networkPermissions.ContainsKey(Normalizer.NormalizeUrl(x)));
}