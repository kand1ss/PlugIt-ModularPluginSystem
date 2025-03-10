using System.Reflection;
using System.Runtime.Loader;
using ModularPluginAPI.Context;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyLoader : IAssemblyLoader
{
    private readonly string _pluginsSource;
    private readonly Dictionary<string, PluginLoadContext> _pluginContexts = new();
    
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
        if(!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Assembly '{assemblyName}' does not exist.");
    }
    
    private string ConcatPathAndName(string name)
    {
        var fullPath = Path.Combine(_pluginsSource, name);
        if(!fullPath.EndsWith(".dll"))
            fullPath += ".dll";
        
        return fullPath;
    }

    
    
    public Assembly GetAssembly(string assemblyName)
    {
        var assemblyPath = ConcatPathAndName(assemblyName);
        CheckFileExists(assemblyName, assemblyPath);
        
        var context = new PluginLoadContext(assemblyPath);
        _pluginContexts.Add(assemblyName, context);
        
        return context.LoadAssembly(assemblyPath);
    }

    public IEnumerable<Assembly> GetAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.Select(GetAssembly);

    public IEnumerable<Assembly> GetAllAssemblies()
    {
        var dllFiles = Directory.GetFiles(_pluginsSource, "*.dll");
        return GetAssemblies(dllFiles);
    }

    
    

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
        var context = _pluginContexts[assemblyName];
        _pluginContexts.Remove(assemblyName);
        Unload(context);
    }

    public void UnloadAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.ToList().ForEach(UnloadAssembly);

    public void UnloadAllAssemblies()
    {
        var keys = _pluginContexts.Keys.ToList();
        keys.ForEach(UnloadAssembly);
    }
}