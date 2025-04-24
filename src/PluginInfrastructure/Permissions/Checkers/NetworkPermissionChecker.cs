using System.Diagnostics.CodeAnalysis;
using PluginAPI.Models.Permissions;
using PluginInfrastructure.Permissions.Checkers.Interfaces;

namespace PluginInfrastructure.Network_Service;

public class NetworkPermissionChecker(IReadOnlyDictionary<string, NetworkPermission> permissions) : PermissionChecker<NetworkPermission>
{
    public override bool CheckPermissionExists(string path, [NotNullWhen(true)] out NetworkPermission? permission)
        => permissions.TryGetValue(Normalizer.NormalizeUrl(path), out permission);
}