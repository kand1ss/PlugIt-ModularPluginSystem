using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IDependencyResolverService
{
    void Resolve(IPluginData plugin);
    IEnumerable<IPluginData> ResolveWithResult(IPluginData plugin);
}