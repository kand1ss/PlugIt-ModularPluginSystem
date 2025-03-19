using System.Reflection;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyLoader
{
    IEnumerable<Assembly> LoadAllAssemblies();
    Assembly LoadAssembly(string assemblyName);
    IEnumerable<string> GetAllAssembliesNames();
    
    void UnloadAssembly(string assemblyName);
}