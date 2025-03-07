using System.Reflection;

namespace ModularPluginAPI.Components;

public interface IPluginExtractor
{
    IEnumerable<Assembly> GetAllFromDirectory();
    Assembly GetFromDirectory(string assemblyName);
    IEnumerable<Assembly> GetFromDirectory(IEnumerable<string> assemblyNames);
}