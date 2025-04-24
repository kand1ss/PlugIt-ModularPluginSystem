using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Services.Interfaces;
using ModularPluginAPI.Services.Plugin_Configurator.Interfaces;
using PluginAPI;
using PluginAPI.Models.Permissions;
using PluginAPI.Services;
using PluginInfrastructure;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class InjectorService(SecuritySettings settings, IPluginPermissionSecurityService permissionSecurity, PluginLoggingFacade logger) : IInjectorService
{
    public void Inject(IPlugin plugin)
    {
        if (plugin is not IConfigurablePlugin)
            return;
        
        TryInjectFileSystemService(plugin);
        TryInjectNetworkService(plugin);
    }

    private void TryInjectFileSystemService(IPlugin plugin)
    {
        if (plugin is not FilePluginBase filePlugin || filePlugin.Configuration is null)
            return;

        var permissionController = new PermissionController<FileSystemPermission>();
        var userPermissions = permissionSecurity.GetFileSystemPermissions();
        var permissionChecker = new FileSystemPermissionChecker(userPermissions);

        foreach (var path in filePlugin.Configuration.Permissions.FileSystemPaths)
        {
            if (permissionChecker.CheckPermissionExists(path, out var permission))
                permissionController.AddPermission(permission);
            else
                logger.InjectionFailed(filePlugin.Name, filePlugin.Version, nameof(PluginFileSystemService), path);
        }

        var service = new PluginFileSystemService(permissionController, settings.FileSystem);
        filePlugin.InjectService(service);
        logger.PluginServiceInjected(filePlugin.Name, filePlugin.Version, nameof(PluginFileSystemService));
    }

    private void TryInjectNetworkService(IPlugin plugin)
    {
        if (plugin is not NetworkPluginBase networkPlugin || networkPlugin.Configuration is null)
            return;

        var permissionController = new PermissionController<NetworkPermission>();
        var userPermissions = permissionSecurity.GetNetworkPermissions();
        var permissionChecker = new NetworkPermissionChecker(userPermissions);

        foreach (var path in networkPlugin.Configuration.Permissions.NetworkURLs)
        {
            if (permissionChecker.CheckPermissionExists(path, out var permission))
                permissionController.AddPermission(permission);
            else
                logger.InjectionFailed(networkPlugin.Name, networkPlugin.Version, nameof(PluginNetworkService), path);
        }

        var service = new PluginNetworkService(permissionController, settings.Network);
        networkPlugin.InjectService(service);
        logger.PluginServiceInjected(networkPlugin.Name, networkPlugin.Version,nameof(PluginNetworkService));
    }
}