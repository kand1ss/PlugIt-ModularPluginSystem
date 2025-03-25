using System.Reflection;
using System.Runtime.Loader;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Context;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyLoader : IAssemblyLoader
{
    private readonly PluginLoggingFacade _logger;
    private readonly Dictionary<string, PluginLoadContext> _assemblyLoadContexts = new();
    
    private string _pluginsSource;

    public AssemblyLoader(PluginLoggingFacade logger, string pluginsSource)
    {
        CheckDirectoryPath(pluginsSource);
        _pluginsSource = pluginsSource;
        _logger = logger;
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


    public ExecutionResult ChangeSource(string pluginDirectory)
    {
        if (!Directory.Exists(pluginDirectory))
            return ExecutionResult.Failure($"Directory '{pluginDirectory}' does not exist.");
        
        _pluginsSource = pluginDirectory;
        _logger.DirectoryChanged(pluginDirectory);
        
        return ExecutionResult.Success();
    }


    public Assembly LoadAssembly(string assemblyName)
    {
        var assemblyPath = ConcatPathAndName(assemblyName);
        if (_assemblyLoadContexts.TryGetValue(assemblyName, out var pluginContext))
            return pluginContext.LoadAssembly(assemblyPath);

        CheckFileExists(assemblyName, assemblyPath);
        var context = new PluginLoadContext(assemblyPath);
        _assemblyLoadContexts.Add(assemblyName, context);

        var assembly = context.LoadAssembly(assemblyPath);
        _logger.AssemblyLoaded(assemblyName);
        return assembly;
    }

    public IEnumerable<string> GetAllAssembliesNames()
        => Directory.GetFiles(_pluginsSource, "*.dll")
            .Select(Path.GetFileNameWithoutExtension!);

    public IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyNames)
        => assemblyNames.Select(LoadAssembly);

    public IEnumerable<Assembly> LoadAllAssemblies()
    {
        var assemblyNames = GetAllAssembliesNames();
        return LoadAssemblies(assemblyNames);
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
        if (!_assemblyLoadContexts.Remove(assemblyName, out var context))
            return;
        
        Unload(context);    
        _logger.AssemblyUnloaded(assemblyName);
    }
}