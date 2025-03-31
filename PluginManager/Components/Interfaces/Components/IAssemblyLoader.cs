using System.Reflection;

namespace ModularPluginAPI.Components;

public interface IAssemblyLoader
{
    Assembly LoadAssembly(string assemblyPath);
    IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyPaths);
    
    void UnloadAssembly(string assemblyName);
}