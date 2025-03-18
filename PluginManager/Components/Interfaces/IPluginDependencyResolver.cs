using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginDependencyResolver
{
    void LoadDependencies(IPlugin plugin);
}