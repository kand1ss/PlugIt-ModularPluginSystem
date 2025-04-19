using PluginAPI;

namespace ModularPluginAPI.Components.Plugin_Configurator.Interfaces;

public interface IPluginConfiguratorService
{
    void Configure(IPlugin plugin);
}