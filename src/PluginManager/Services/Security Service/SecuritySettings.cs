using PluginAPI.Services;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class SecuritySettings
{
    public NetworkServiceSettings Network { get; } = new();
    public FileSystemServiceSettings FileSystem { get; } = new();
}