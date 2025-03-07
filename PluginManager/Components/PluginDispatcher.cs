
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginDispatcher(IRepository repository, IAssemblyExtractor extractor, IAssemblyHandler handler)
{
    public void ExtractAndSavePlugins(string dllName)
    {
        var extractedAssembly = extractor.GetAssembly(dllName);
        var plugins = handler.GetAllPlugins(extractedAssembly);
        repository.AddRange(plugins);
        extractor.Clear();
    }

    public void ExtractAndSavePlugins(IEnumerable<string> dllNames)
    {
        var extractedAssemblies = extractor.GetAssemblies(dllNames);
        var plugins = handler.GetAllPlugins(extractedAssemblies);
        repository.AddRange(plugins);
        extractor.Clear();
    }
}