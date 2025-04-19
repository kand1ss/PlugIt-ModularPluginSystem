using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IDependencyResolverService
{
    void Resolve(IPlugin plugin);
    IEnumerable<IPlugin> ResolveWithResult(IPlugin plugin);
}