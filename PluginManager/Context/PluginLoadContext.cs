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
    
    public Assembly LoadAssembly(string assemblyName)
        => LoadFromAssemblyPath(Path.Combine(path, assemblyName));
}