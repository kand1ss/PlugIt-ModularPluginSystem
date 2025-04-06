using System.Reflection;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IAssemblyHandler
{
    public T? GetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin;
    public IEnumerable<IPlugin> GetAllPlugins(Assembly assembly);
}