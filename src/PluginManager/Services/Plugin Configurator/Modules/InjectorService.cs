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

        var permissionController = new PermissionController<FileSystemPermission>(Normalizer.NormalizeDirectoryPath);
        var userPermissions = permissionSecurity.GetFileSystemPermissions();
        var permissionChecker = new FileSystemPermissionChecker(userPermissions);

        foreach (var path in filePlugin.Configuration.Permissions.FileSystemPaths)
        {
            if (permissionChecker.CheckPermissionExists(path, out var permission))
                permissionController.AddPermission(permission!);
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

        var permissionController = new PermissionController<NetworkPermission>(Normalizer.NormalizeUrl);
        var userPermissions = permissionSecurity.GetNetworkPermissions();

        foreach (var path in networkPlugin.Configuration.Permissions.NetworkURLs)
        {
            var normalizedPath = Normalizer.NormalizeUrl(path);
            if (!userPermissions.TryGetValue(normalizedPath, out var permission))
                logger.InjectionFailed(networkPlugin.Name, networkPlugin.Version, nameof(PluginNetworkService), path);
            
            permissionController.AddPermission(permission!);
        }

        var service = new PluginNetworkService(permissionController, settings.Network);
        networkPlugin.InjectService(service);
        logger.PluginServiceInjected(networkPlugin.Name, networkPlugin.Version,nameof(PluginNetworkService));
    }
}