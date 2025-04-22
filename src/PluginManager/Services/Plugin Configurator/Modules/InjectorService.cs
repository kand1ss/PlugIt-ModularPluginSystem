using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Services.Plugin_Configurator.Interfaces;
using PluginAPI;
using PluginAPI.Services;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class InjectorService(SecuritySettings settings, PluginLoggingFacade logger) : IInjectorService
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

        var permissionController = new FileSystemPermissionController();
        foreach (var permission in filePlugin.Configuration.Permissions.FileSystemPaths)
            permissionController.AddAllowedDirectory(permission);

        var service = new PluginFileSystemService(permissionController, settings.FileSystem);
        filePlugin.InjectService(service);
        logger.PluginServiceInjected(filePlugin.Name, filePlugin.Version, "FileSystemService");
    }

    private void TryInjectNetworkService(IPlugin plugin)
    {
        if (plugin is not NetworkPluginBase networkPlugin || networkPlugin.Configuration is null)
            return;

        var permissionController = new NetworkPermissionController();
        foreach (var permission in networkPlugin.Configuration.Permissions.NetworkURLs)
            permissionController.AddAllowedUrl(permission);

        var service = new PluginNetworkService(permissionController, settings.Network);
        networkPlugin.InjectService(service);
        logger.PluginServiceInjected(networkPlugin.Name, networkPlugin.Version,"NetworkService");
    }
}