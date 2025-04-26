using PluginAPI.Services;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public interface ISecuritySettings
{
    public NetworkServiceSettings Network { get; }
    public FileSystemServiceSettings FileSystem { get; }
}