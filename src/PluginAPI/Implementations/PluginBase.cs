
namespace PluginAPI;

public abstract class PluginBase : IPluginWithDependencies
{
    public Dictionary<string, IPluginData> LoadedDependencies { get; } = new();
    
    public abstract string Name { get; }
    public abstract Version Version { get; }
    public PluginConfiguration? Configuration { get; private set; }
    public abstract string Author { get; }

    public void LoadConfiguration(PluginConfiguration? configuration)
        => Configuration = configuration;
    
    public void LoadDependency(IPluginData plugin)
        => LoadedDependencies.Add(plugin.Name, plugin);
    public void LoadDependencies(IEnumerable<IPluginData> plugins)
    {
        foreach(var plugin in plugins)
            LoadDependency(plugin);
    }
    public T GetDependencyPlugin<T>(string pluginName) where T : IPluginData
    {
        if(LoadedDependencies.TryGetValue(pluginName, out IPluginData? plugin))
        {
            if (plugin is T typedPlugin)
                return typedPlugin;
            
            throw new InvalidCastException($"A plugin named {pluginName} is not of type {typeof(T).Name}.");
        }
        throw new KeyNotFoundException($"A plugin named {pluginName} is not found.");
    }
}