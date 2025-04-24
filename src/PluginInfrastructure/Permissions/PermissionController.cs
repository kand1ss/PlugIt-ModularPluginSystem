using PluginAPI.Models.Permissions;
using PluginAPI.Models.Permissions.Interfaces;
using PluginInfrastructure.Normalization;

namespace PluginInfrastructure;

public class PermissionController<T> : IPermissionController<T> where T : PermissionBase
{
    private readonly Dictionary<string, T> _permissions = new();
    
    private static readonly Dictionary<Type, IPathNormalizer> _normalizers = new()
    {
        { typeof(FileSystemPermission), new FilePathNormalizer() },
        { typeof(NetworkPermission), new NetworkPathNormalizer() }
    };
    
    private readonly IPathNormalizer _normalizer = _normalizers.TryGetValue(typeof(T), out var norm)
        ? norm : throw new InvalidOperationException($"No normalizer registered for type {typeof(T)}");
    
    public void AddPermission(T permission)
    {
        var normalized = _normalizer.Normalize(permission.Path);
        _permissions.TryAdd(normalized, permission);
    }

    public IReadOnlyCollection<T> GetPermissions()
        => _permissions.Values;
}