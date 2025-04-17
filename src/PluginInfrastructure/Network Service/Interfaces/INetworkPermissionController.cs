using PluginAPI.Models.Permissions;

namespace PluginInfrastructure.Network_Service.Interfaces;

public interface INetworkPermissionController
{
    void AddAllowedUrl(string url, NetworkPermission permission);
    IDictionary<string, NetworkPermission> GetAllowedUrls();
}