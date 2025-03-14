using System.Reflection;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyLoader
{
    IEnumerable<Assembly> LoadAllAssemblies();
    Assembly LoadAssembly(string assemblyName);
    string GetPluginPath();
    
    void UnloadAssembly(string assemblyName);
}