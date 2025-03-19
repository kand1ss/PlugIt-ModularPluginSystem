using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginDependencyResolver
{
    void Resolve(IPlugin plugin);
}