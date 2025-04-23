using PluginAPI.Models.Permissions;
using PluginInfrastructure.Permissions.Checkers.Interfaces;

namespace PluginInfrastructure.Network_Service;

public class NetworkPermissionChecker(IReadOnlyDictionary<string, NetworkPermission> permissions) : PermissionChecker<NetworkPermission>
{
    public override bool CheckPermissionAllow(string path, out NetworkPermission? permission)
        => permissions.TryGetValue(Normalizer.NormalizeUrl(path), out permission);
}