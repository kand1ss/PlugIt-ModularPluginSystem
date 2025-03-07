using System.Reflection;
using System.Runtime.Loader;

namespace ModularPluginAPI.Context;

public class PluginLoadContext(string path) : AssemblyLoadContext(isCollectible: true)
{
    private readonly AssemblyDependencyResolver _resolver = new(path);
    
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath is not null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
    
    public Assembly LoadAssembly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Assembly not found: {assemblyPath}");
        
        return LoadFromAssemblyPath(assemblyPath);
    }
    
    public IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyPaths)
        => assemblyPaths.Select(LoadFromAssemblyPath);
}