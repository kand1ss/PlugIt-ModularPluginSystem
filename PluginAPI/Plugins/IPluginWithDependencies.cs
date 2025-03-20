using PluginAPI.Dependency;

namespace PluginAPI;

public interface IPluginWithDependencies : IConfigurablePlugin
{
    Dictionary<string, IPlugin> LoadedDependencies { get; }
    void LoadDependency(IPlugin plugin);
    void LoadDependencies(IEnumerable<IPlugin> plugins);
    T GetDependencyPlugin<T>(string pluginName) where T : IPlugin;
}