using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IDependencyResolverService
{
    void Resolve(IPlugin plugin);
}