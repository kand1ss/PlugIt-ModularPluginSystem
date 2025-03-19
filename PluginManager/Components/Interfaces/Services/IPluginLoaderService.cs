using System.Reflection;
using PluginAPI;

namespace ModularPluginAPI.Components.Interfaces.Services;

public interface IPluginLoaderService
{
    Assembly LoadAssemblyByPluginName(string pluginName);
    T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin;
}