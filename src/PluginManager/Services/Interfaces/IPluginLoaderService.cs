using System.Reflection;
using PluginAPI;

namespace ModularPluginAPI.Components.Interfaces.Services;

public interface IPluginLoaderService
{
    Assembly LoadAssembly(string assemblyName);
    Assembly LoadAssemblyByPluginName(string pluginName);
    void UnloadAssembly(string assemblyName);
    void UnloadAssemblyByPluginName(string pluginName);
    T TryGetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPluginData;
}