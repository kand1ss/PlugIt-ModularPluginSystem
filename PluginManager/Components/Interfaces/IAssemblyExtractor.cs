using System.Reflection;

namespace ModularPluginAPI.Components;

public interface IAssemblyExtractor
{
    IEnumerable<Assembly> GetAllFromDirectory();
    Assembly GetAssembly(string assemblyName);
    IEnumerable<Assembly> GetAssemblies(IEnumerable<string> assemblyNames);

    void ClearAssemblies();
}