using System.Reflection;
using ModularPluginAPI.Context;

namespace ModularPluginAPI.Components;

public class AssemblyExtractor : IAssemblyExtractor
{
    private readonly string _pluginsSource;
    private PluginLoadContext _context;
    
    public AssemblyExtractor(string pluginsSource)
    {
        if (string.IsNullOrWhiteSpace(pluginsSource) || !Directory.Exists(pluginsSource))
            throw new ArgumentException("Invalid plugins path.", nameof(pluginsSource));

        _pluginsSource = pluginsSource;
        _context = new PluginLoadContext(pluginsSource);
    }
    
    private string ConcatPathAndName(string name)
    {
        var fullPath = Path.Combine(_pluginsSource, name);
        if(!fullPath.EndsWith(".dll"))
            fullPath += ".dll";
        
        return fullPath;
    }

    public IEnumerable<Assembly> GetAllFromDirectory()
    {
        var dllFiles = Directory.GetFiles(_pluginsSource, "*.dll");
        return _context.LoadAssemblies(dllFiles);
    }

    public Assembly GetAssembly(string assemblyName)
    {
        var assemblyPath = ConcatPathAndName(assemblyName);
        if(!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Assembly '{assemblyName}' does not exist.");
        
        return _context.LoadAssembly(assemblyPath);
    }

    public IEnumerable<Assembly> GetAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.Select(GetAssembly);

    public void ClearAssemblies()
    {
        _context.Unload();
        _context = new PluginLoadContext(_pluginsSource);
    }
}