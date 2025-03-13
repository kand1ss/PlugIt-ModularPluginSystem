using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;

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
        var pluginLifecycleManager = new PluginLifecycleManager();
        var pluginExecutor = new PluginExecutor(pluginLifecycleManager);

        _dispatcher = new(repository, pluginExtractor, assemblyHandler, 
            pluginExecutor, pluginLifecycleManager);
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
    /// Launches a standard plugin by its name. 
    /// After execution, the assembly containing the plugin is unloaded if it is no longer in use.
    /// </summary>
    /// <param name="pluginName">The name of the standard plugin to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if the specified plugin is not found.</exception>
    public void ExecutePlugin(string pluginName)
    {
        _dispatcher.StartPlugin(pluginName);
        _dispatcher.UnloadAssemblyByPluginName(pluginName);
    }
    
    /// <summary>
    /// Runs all standard plugins from the specified assembly. 
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly containing the standard plugins.</param>
    /// <exception cref="AssemblyNotFoundException">Thrown if the specified assembly is not found.</exception>
    public void ExecutePluginsFromAssembly(string assemblyName)
    {
        _dispatcher.StartAllPluginsFromAssembly(assemblyName);
        _dispatcher.UnloadAssembly(assemblyName);
    }

    /// <summary>
    /// Runs all standard plugins from all detected assemblies in the plugin directory. 
    /// After execution, each assembly is unloaded if it is no longer referenced.
    /// </summary>
    public void ExecuteAllPlugins()
    {
        _dispatcher.StartAllPlugins();
        _dispatcher.UnloadAllAssemblies();
    }

    /// <summary>
    /// Launches an extension plugin by its name and allows it to modify the provided data.
    /// Extension plugins are designed to process and modify external data objects.
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <typeparam name="T">The type of data being processed by the extension plugin.</typeparam>
    /// <param name="data">Reference to the data object to be modified by the plugin.</param>
    /// <param name="pluginName">The name of the extension plugin to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if the specified plugin is not found.</exception>
    public void ExecuteExtensionPlugin<T>(ref T data, string pluginName)
    {
        _dispatcher.StartExtensionPlugin(ref data, pluginName);
        _dispatcher.UnloadAssemblyByPluginName(pluginName);
    }

    /// <summary>
    /// Launches multiple extension plugins by their names and allows them to modify the provided data. 
    /// Each plugin is executed sequentially, and the data may be modified by multiple plugins.
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <typeparam name="T">The type of data being processed by the extension plugins.</typeparam>
    /// <param name="data">Reference to the data object to be modified by the plugins.</param>
    /// <param name="pluginNames">The names of the extension plugins to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if any of the specified plugins are not found.</exception>
    public void ExecuteExtensionPlugin<T>(ref T data, IEnumerable<string> pluginNames)
    {
        foreach (var pluginName in pluginNames)
            ExecuteExtensionPlugin(ref data, pluginName);
    }

    /// <summary>
    /// Executes a network plugin by its name, optionally sending data and expecting a response.
    /// Network plugins handle remote data processing and communication.
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <param name="pluginName">The name of the network plugin to be executed.</param>
    /// <param name="expectResponse">
    /// <para>
    /// <c>true</c> if a response is expected from the plugin.  
    /// <c>false</c> if no response is expected.
    /// </para>
    /// </param>
    /// <param name="requestData">The data to be sent to the plugin.
    /// If no data should be sent, pass <c>null</c>.</param>
    /// <returns>The response data received from the plugin, or <c>null</c> if no response is expected.</returns>
    public byte[]? ExecuteNetworkPlugin(string pluginName, bool expectResponse, byte[]? requestData = null)
    {
        if (requestData is not null)
            _dispatcher.SendNetworkPlugin(pluginName, requestData);

        var response = expectResponse ? _dispatcher.ReceiveNetworkPlugin(pluginName) : null;
        _dispatcher.UnloadAssemblyByPluginName(pluginName);

        return response;
    }
    
    /// <summary>
    /// Retrieves the state of a specific plugin by its name.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose state is to be retrieved.</param>
    /// <returns>The current state of the specified plugin as a string.</returns>
    public string GetPluginState(string pluginName)
        => _dispatcher.GetPluginState(pluginName);

    /// <summary>
    /// Retrieves the states of all plugins.
    /// </summary>
    /// <returns>An enumerable collection of PluginInfo objects representing the states of all plugins.</returns>
    public IEnumerable<PluginInfo> GetPluginsStates()
        => _dispatcher.GetPluginStates();
}