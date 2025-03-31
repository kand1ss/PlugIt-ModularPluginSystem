using System.Reflection;

namespace ModularPluginAPI.Components;

public interface IAssemblyLoader
{
    void ChangeSource(string pluginDirectory);
    
    IEnumerable<Assembly> LoadAllAssemblies();
    Assembly LoadAssembly(string assemblyName);
    IEnumerable<string> GetAllAssembliesNames();
    
    void UnloadAssembly(string assemblyName);
}