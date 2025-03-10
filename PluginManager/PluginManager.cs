using ModularPluginAPI.Components;
using PluginAPI;

namespace ModularPluginAPI;

public class PluginManager : IDisposable
{
    private readonly PluginDispatcher _dispatcher;
    private readonly FileSystemWatcher _fileWatcher = new();

    public PluginManager(string pluginsSource)
    {
        var assemblyHandler = new AssemblyHandler();
        var pluginExtractor = new AssemblyLoader(pluginsSource);
        var repository = new AssemblyMetadataRepository();
        var pluginExecutor = new PluginExecutor();
        
        _dispatcher = new(repository, pluginExtractor, assemblyHandler, pluginExecutor);
        _dispatcher.RebuildMetadata();
        
        InitializeFileWatcher(pluginsSource);
    }

    private void InitializeFileWatcher(string source)
    {
        _fileWatcher.Path = source;
        _fileWatcher.Filter = "*.dll";
        
        _fileWatcher.Changed += DirectoryChanged;
        _fileWatcher.Created += DirectoryChanged;
        _fileWatcher.Deleted += DirectoryChanged;
        
        _fileWatcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
        _fileWatcher.Changed -= DirectoryChanged;
        _fileWatcher.Created -= DirectoryChanged;
        _fileWatcher.Deleted -= DirectoryChanged;
        
        _fileWatcher.Dispose();
        GC.SuppressFinalize(this);
    }

    private void DirectoryChanged(object obj, FileSystemEventArgs e) 
        => _dispatcher.RebuildMetadata();
    
    /// <summary>
    /// Launches a plugin by its name. 
    /// After execution, the assembly containing the plugin is unloaded if it is no longer in use.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if the specified plugin is not found.</exception>
    public void StartPlugin(string pluginName)
    {
        _dispatcher.StartPlugin(pluginName);
        _dispatcher.UnloadAssemblyByPluginName(pluginName);
    }

    /// <summary>
    /// Runs all plugins from the specified assembly. 
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly containing the plugins.</param>
    /// <exception cref="AssemblyNotFoundException">Thrown if the specified assembly is not found.</exception>
    public void StartAllPluginsFromAssembly(string assemblyName)
    {
        _dispatcher.StartAllPluginsFromAssembly(assemblyName);
        _dispatcher.UnloadAssembly(assemblyName);
    }

    /// <summary>
    /// Runs all plugins from all detected assemblies in the plugin directory. 
    /// After execution, each assembly is unloaded if it is no longer referenced.
    /// </summary>
    public void StartAllPlugins()
    {
        _dispatcher.StartAllPlugins();
        _dispatcher.UnloadAllAssemblies();
    }

    /// <summary>
    /// Launches an extension plugin by its name and allows it to modify the provided data. 
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <typeparam name="T">The type of data being processed by the plugin.</typeparam>
    /// <param name="data">Reference to the data object to be modified by the plugin.</param>
    /// <param name="pluginName">The name of the plugin to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if the specified plugin is not found.</exception>
    public void StartExtensionPlugin<T>(ref T data, string pluginName)
    {
        _dispatcher.StartExtensionPlugin(ref data, pluginName);
        _dispatcher.UnloadAssemblyByPluginName(pluginName);
    }
}