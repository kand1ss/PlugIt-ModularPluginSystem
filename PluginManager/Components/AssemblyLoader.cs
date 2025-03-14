using System.Reflection;
using System.Runtime.Loader;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Context;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyLoader : IAssemblyLoader
{
    private readonly string _pluginsSource;
    private readonly Dictionary<string, PluginLoadContext> _assemblyLoadContexts = new();
    
    public AssemblyLoader(string pluginsSource)
    {
        CheckDirectoryPath(pluginsSource);
        _pluginsSource = pluginsSource;
    }

    private static void CheckDirectoryPath(string pluginsSource)
    {
        if (string.IsNullOrWhiteSpace(pluginsSource) || !Directory.Exists(pluginsSource))
            throw new ArgumentException("Invalid plugins path.", nameof(pluginsSource));
    }
    private static void CheckFileExists(string assemblyName, string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new AssemblyNotFoundException(assemblyName);
    }
    
    private string ConcatPathAndName(string name)
    {
        var fullPath = Path.Combine(_pluginsSource, name);
        if(!fullPath.EndsWith(".dll"))
            fullPath += ".dll";
        
        return fullPath;
    }

    
    
    public Assembly LoadAssembly(string assemblyName)
    {
        var assemblyPath = ConcatPathAndName(assemblyName);
        if (_assemblyLoadContexts.TryGetValue(assemblyName, out var pluginContext))
            return pluginContext.LoadAssembly(assemblyPath);

        CheckFileExists(assemblyName, assemblyPath);
        var context = new PluginLoadContext(assemblyPath);
        _assemblyLoadContexts.Add(assemblyName, context);
        
        return context.LoadAssembly(assemblyPath);
    }

    public IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.Select(LoadAssembly);

    public IEnumerable<Assembly> LoadAllAssemblies()
    {
        var assemblies = Directory.GetFiles(_pluginsSource, "*.dll");
        var assemblyNames = assemblies.Select(Path.GetFileNameWithoutExtension);
        
        return LoadAssemblies(assemblyNames!);
    }


    public string GetPluginPath() => _pluginsSource;
    

    private void Unload(AssemblyLoadContext context)
    {
        var weakReference = new WeakReference(context);
        context.Unload();
        
        for (int i = 0; weakReference.IsAlive && (i < 5); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
    
    public void UnloadAssembly(string assemblyName)
    {
        if (!_assemblyLoadContexts.Remove(assemblyName, out var context))
            return;
        Unload(context);
    }
}