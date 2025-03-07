using System.Reflection;
using ModularPluginAPI.Context;

namespace ModularPluginAPI.Components;

public class AssemblyExtractor : IAssemblyExtractor
{
    private readonly string _pluginsSource;
    private PluginLoadContext _context;
    
    public AssemblyExtractor(string pluginsSource)
    {
        if (string.IsNullOrWhiteSpace(pluginsSource))
            throw new ArgumentException("Plugins path cannot be null or empty.", nameof(pluginsSource));
        
        _pluginsSource = pluginsSource;
        _context = new PluginLoadContext(pluginsSource);
    }
    
    private string ConcatPathAndName(string name)
        => Path.Combine(_pluginsSource, name + ".dll");
    
    public IEnumerable<Assembly> GetAllFromDirectory()
    {
        var dllFiles = Directory.GetFiles(_pluginsSource, "*.dll");
        return _context.LoadAssemblies(dllFiles);
    }

    public Assembly GetAssembly(string assemblyName)
    {
        var assemblyPath = ConcatPathAndName(assemblyName);
        return _context.LoadAssembly(assemblyPath);
    }

    public IEnumerable<Assembly> GetAssemblies(IEnumerable<string> assemblyNames)
    {
        var assemblyPaths = assemblyNames.Select(ConcatPathAndName);
        return _context.LoadAssemblies(assemblyPaths);
    }

    public void Clear()
    {
        _context.Unload();
        _context = new PluginLoadContext(_pluginsSource);
    }
}