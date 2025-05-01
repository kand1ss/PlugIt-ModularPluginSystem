
namespace PluginAPI;

public interface IPluginWithDependencies : IConfigurablePlugin
{
    Dictionary<string, IPluginData> LoadedDependencies { get; }
    void LoadDependency(IPluginData plugin);
    void LoadDependencies(IEnumerable<IPluginData> plugins);
    T GetDependencyPlugin<T>(string pluginName) where T : IPluginData;
}