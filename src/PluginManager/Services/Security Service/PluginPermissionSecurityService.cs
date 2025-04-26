using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using PluginAPI.Services;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class PluginPermissionSecurityService(SecuritySettingsProvider settingsProvider) : IPluginPermissionSecurityService
{
    public bool CheckSafety(PluginMetadata pluginMetadata)
    {
        var permissions = pluginMetadata.Configuration.Permissions;
        return 
            CheckFileSystemPermissions(permissions.FileSystemPaths) && 
            CheckNetworkPermissions(permissions.NetworkURLs);
    }

    private bool CheckFileSystemPermissions(IEnumerable<string> filePaths)
    {
        var filePermissionChecker = new FileSystemPermissionChecker(settingsProvider.Settings.FileSystemPermissions);
        return filePermissionChecker.CheckPermissionsExists(filePaths.ToList());
    }

    private bool CheckNetworkPermissions(IEnumerable<string> networkPaths)
    {
        var networkPermissionChecker = new NetworkPermissionChecker(settingsProvider.Settings.NetworkPermissions);
        return networkPermissionChecker.CheckPermissionsExists(networkPaths.ToList());
    }
}