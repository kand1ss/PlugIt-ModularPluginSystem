using PluginAPI.Models.Permissions;
using PluginInfrastructure.Network_Service.Interfaces;

namespace PluginInfrastructure.Network_Service;

public class NetworkPermissionController : INetworkPermissionController
{
    private readonly Dictionary<string, NetworkPermission> _allowedUrls = new();

    public void AddAllowedUrl(string url, NetworkPermission? permission = null)
    {
        permission ??= new NetworkPermission();
        _allowedUrls.Add(Normalizer.NormalizeUrl(url), permission);
    }

    public IDictionary<string, NetworkPermission> GetAllowedUrls()
        => _allowedUrls;
}