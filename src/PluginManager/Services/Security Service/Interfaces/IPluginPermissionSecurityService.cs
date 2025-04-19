using ModularPluginAPI.Models;
using PluginAPI.Models.Permissions;

namespace ModularPluginAPI.Services.Interfaces;

public interface IPluginPermissionSecurityService
{
    void AddFileSystemPermission(string normalizedPath, bool canRead, bool canWrite, bool recursive);
    void AddNetworkPermission(string normalizedUrl, bool canGet, bool canPost);
    
    IDictionary<string, FileSystemPermission> GetFileSystemPermissions();
    IDictionary<string, NetworkPermission> GetNetworkPermissions();
    
    bool CheckSafety(PluginMetadata pluginMetadata);
}