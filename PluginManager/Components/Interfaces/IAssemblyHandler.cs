using System.Reflection;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IAssemblyHandler
{
    public IEnumerable<IPlugin> GetAllPlugins(Assembly assembly);
    public IEnumerable<IPlugin> GetAllPlugins(IEnumerable<Assembly> assemblies);
    public IEnumerable<T> GetPlugins<T>(Assembly assembly) where T : class;
    public IEnumerable<T> GetPlugins<T>(IEnumerable<Assembly> assemblies) where T : class;
}