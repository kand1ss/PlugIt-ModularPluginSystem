using ModularPluginAPI.Models;
using PluginAPI.Models.Permissions;

namespace ModularPluginAPI.Services.Interfaces;

public interface IPluginPermissionSecurityService
{
    void AddFileSystemPermission(string fullPath, bool canRead, bool canWrite);
    void AddNetworkPermission(string url, bool canGet, bool canPost);
    
    IDictionary<string, FileSystemPermission> GetFileSystemPermissions();
    IDictionary<string, NetworkPermission> GetNetworkPermissions();
    
    bool CheckSafety(PluginMetadata pluginMetadata);
}