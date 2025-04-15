using System.Text.RegularExpressions;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;

namespace ModularPluginAPI.Components;

public class PluginPermissionSecurityService : IPluginPermissionSecurityService
{
    private readonly List<string> _fileSystemPermissions = new();
    private readonly List<string> _networkPermissions = new();
    
    
    public void AddFileSystemPermission(string fullPath)
    {
        if (_fileSystemPermissions.Contains(fullPath))
            return;
        
        _fileSystemPermissions.Add(fullPath);
    }

    public void AddNetworkPermission(string url)
    {
        if (_networkPermissions.Contains(url))
            return;

        var regularExpression = new Regex("^https?:\\/\\/[^\\s\\/$.?#].[^\\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (!regularExpression.IsMatch(url))
            return;
        
        _networkPermissions.Add(url);
    }
    
    public IEnumerable<string> GetFileSystemPermissions()
        => _fileSystemPermissions;

    public IEnumerable<string> GetNetworkPermissions()
        => _networkPermissions;

    
    public bool CheckSafety(PluginMetadata pluginMetadata)
    {
        var permissions = pluginMetadata.Configuration.Permissions;
        return 
            CheckFileSystemPermissions(permissions.FileSystemPaths) && 
            CheckNetworkPermissions(permissions.NetworkURLs);
    }


    private bool CheckFileSystemPermissions(IEnumerable<string> pluginPaths)
        => pluginPaths.All(_fileSystemPermissions.Contains);

    private bool CheckNetworkPermissions(IEnumerable<string> pluginUrls)
        => pluginUrls.All(_networkPermissions.Contains);

}