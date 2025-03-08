using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IRepository
{
    void Add(IPlugin plugin);
    void AddRange(IEnumerable<IPlugin> plugins);
    void Remove(string pluginName);

    IEnumerable<IPlugin> GetAll();
    IEnumerable<T> GetAll<T>() where T : IPlugin;
    IPlugin? GetByName(string pluginName);
    T? GetByName<T>(string pluginName) where T : IPlugin;
}