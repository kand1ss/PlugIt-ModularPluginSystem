namespace PluginAPI.Models.Permissions.Interfaces;

public interface IPermissionController<T> where T : PermissionBase
{
    void AddPermission(T permission);
    IReadOnlyCollection<T> GetPermissions();
}