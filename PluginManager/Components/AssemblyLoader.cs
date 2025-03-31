using System.Reflection;
using System.Runtime.Loader;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Context;
using ModularPluginAPI.Exceptions;

namespace ModularPluginAPI.Components;

public class AssemblyLoader(PluginLoggingFacade logger) : IAssemblyLoader
{
    private readonly Dictionary<string, PluginLoadContext> _assemblyLoadContexts = new();
    
    private static void CheckFileExists(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new AssemblyNotFoundException(assemblyPath);
    }

    public Assembly LoadAssembly(string assemblyPath)
    {
        if (_assemblyLoadContexts.TryGetValue(assemblyPath, out var pluginContext))
            return pluginContext.LoadAssembly(assemblyPath);

        CheckFileExists(assemblyPath);
        var context = new PluginLoadContext(assemblyPath);
        _assemblyLoadContexts.Add(assemblyPath, context);

        var assembly = context.LoadAssembly(assemblyPath);
        logger.AssemblyLoaded(assemblyPath);
        return assembly;
    }

    public IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyPaths)
        => assemblyPaths.Select(LoadAssembly);

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
        logger.AssemblyUnloaded(assemblyName);
    }
}