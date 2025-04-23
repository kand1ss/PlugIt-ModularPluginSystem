using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using PluginAPI.Models.Permissions;
using PluginAPI.Services;
using PluginInfrastructure;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class PluginPermissionSecurityService : IPluginPermissionSecurityService
{
    private readonly Dictionary<string, FileSystemPermission> _fileSystemPermissions = new();
    private readonly Dictionary<string, NetworkPermission> _networkPermissions = new();
    
    private readonly FileSystemPermissionChecker _filePermissionChecker;
    private readonly NetworkPermissionChecker _networkPermissionChecker;
    
    private readonly PluginLoggingFacade _logger;

    public PluginPermissionSecurityService(PluginLoggingFacade logger)
    {
        _filePermissionChecker = new(_fileSystemPermissions);
        _networkPermissionChecker = new(_networkPermissions);
        _logger = logger;
    }
    
    public void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = true)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(fullPath);
        if (_fileSystemPermissions.ContainsKey(normalizedPath))
            return;

        _fileSystemPermissions.Add(normalizedPath, new FileSystemPermission(normalizedPath, canRead, canWrite, recursive));
        _logger.FilePermissionAdded(fullPath, canRead, canWrite);
    }

    public void AddNetworkPermission(string url, bool canGet = true, bool canPost = true)
    {
        var normalizedUrl = Normalizer.NormalizeUrl(url);
        if (_networkPermissions.ContainsKey(normalizedUrl))
            return;

        _networkPermissions.Add(normalizedUrl, new NetworkPermission(normalizedUrl, canGet, canPost));
        _logger.NetworkPermissionAdded(url, canGet, canPost);
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
        => _filePermissionChecker.CheckPermissionsAllow(filePaths.ToList());

    private bool CheckNetworkPermissions(IEnumerable<string> networkPaths)
        => _networkPermissionChecker.CheckPermissionsAllow(networkPaths.ToList());
}