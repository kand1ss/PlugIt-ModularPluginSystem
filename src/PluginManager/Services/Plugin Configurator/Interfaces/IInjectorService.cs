using PluginAPI;

namespace ModularPluginAPI.Services.Plugin_Configurator.Interfaces;

public interface IInjectorService
{
    void Inject(IPlugin plugin);
}