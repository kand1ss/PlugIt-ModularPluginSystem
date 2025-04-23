using PluginAPI.Models.Permissions;
using PluginAPI.Models.Permissions.Interfaces;

namespace PluginInfrastructure;

public class PermissionController<T>(
    Func<string, string> normalizingStrategy) : IPermissionController<T> where T : PermissionBase
{
    private readonly Dictionary<string, T> _permissions = new();
    
    public void AddPermission(T permission)
    {
        var normalized = normalizingStrategy(permission.Path);
        _permissions.TryAdd(normalized, permission);
    }

    public IReadOnlyCollection<T> GetPermissions()
        => _permissions.Values;
}