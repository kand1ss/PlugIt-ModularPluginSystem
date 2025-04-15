using ModularPluginAPI.Models;

namespace ModularPluginAPI.Services.Interfaces;

public interface IPluginPermissionSecurityService
{
    void AddFileSystemPermission(string fullPath);
    void AddNetworkPermission(string url);
    
    IEnumerable<string> GetFileSystemPermissions();
    IEnumerable<string> GetNetworkPermissions();
    
    bool CheckSafety(PluginMetadata pluginMetadata);
}