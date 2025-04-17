using PluginAPI.Services.interfaces;
using PluginInfrastructure.Network_Service.Interfaces;

namespace PluginInfrastructure.Network_Service;

public static class PluginNetworkServiceFactory
{
    public static IPluginNetworkService Create(INetworkPermissionController controller)
        => new PluginNetworkService(controller);
}