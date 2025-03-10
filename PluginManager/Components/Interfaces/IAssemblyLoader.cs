using System.Reflection;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyLoader
{
    IEnumerable<Assembly> GetAllAssemblies();
    Assembly GetAssembly(string assemblyName);
    IEnumerable<Assembly> GetAssemblies(IEnumerable<string> assemblyNames);
    
    void UnloadAssembly(string assemblyName);
    void UnloadAssemblies(IEnumerable<string> assemblyNames);
    void UnloadAllAssemblies();
}