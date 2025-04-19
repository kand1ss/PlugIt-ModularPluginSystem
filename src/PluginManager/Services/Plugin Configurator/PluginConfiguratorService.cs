using ModularPluginAPI.Components.Plugin_Configurator.Interfaces;
using ModularPluginAPI.Services.Plugin_Configurator.Interfaces;
using PluginAPI;

namespace ModularPluginAPI.Components.Plugin_Configurator;

public class PluginConfiguratorService(IDependencyResolverService dependencyResolver, IInjectorService injectorService) 
    : IPluginConfiguratorService
{
    public void Configure(IPlugin plugin)
    {
        var resolvedPlugins = dependencyResolver.ResolveWithResult(plugin);
        injectorService.Inject(plugin);
        foreach (var resolvedPlugin in resolvedPlugins)
            injectorService.Inject(resolvedPlugin);
    }
}