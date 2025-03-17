using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginDependencyResolver
{
    void LoadPlugin(IPlugin plugin);
}