using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginRepository : IRepository
{
    private readonly Dictionary<string, IPlugin> _plugins = new();
    
    public void Add(IPlugin plugin)
        => _plugins.TryAdd(plugin.Name, plugin);

    public void AddRange(IEnumerable<IPlugin> plugins)
    {
        foreach (var plugin in plugins)
            Add(plugin);
    }

    public void Remove(string pluginName) => _plugins.Remove(pluginName);


    public IReadOnlyCollection<IPlugin> GetAll()
        => _plugins.Values;
    public IEnumerable<T> GetAll<T>() where T : IPlugin
        => _plugins.Values.OfType<T>();

    
    public IPlugin? GetByName(string pluginName)
        => _plugins.Values.FirstOrDefault(p => string.Equals(p.Name, pluginName, StringComparison.OrdinalIgnoreCase));
    public T? GetByName<T>(string pluginName) where T : IPlugin
        => _plugins.Values.OfType<T>().FirstOrDefault(
            p => string.Equals(p.Name, pluginName, StringComparison.OrdinalIgnoreCase));
}