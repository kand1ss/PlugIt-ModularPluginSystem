using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Logger.Components;
using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI;

public class PluginManager : IDisposable
{
    private PluginDispatcher _dispatcher;
    private readonly FileSystemWatcher _fileWatcher = new();

    private readonly ILoggerService _logger;
    private readonly IPluginLifecycleManager _lifecycleManager;

    private void Initialize(string pluginsSource, IAssemblyMetadataRepository repository,
        IAssemblyLoader assemblyLoader, IAssemblyHandler assemblyHandler, IPluginExecutor pluginExecutor,
        IPluginLifecycleManager lifecycleManager, PluginLoggingFacade logger,
        IPluginDependencyResolver dependencyResolver)
    {
        _dispatcher = new(repository, assemblyLoader, assemblyHandler,
            pluginExecutor, lifecycleManager, logger, dependencyResolver);
        _dispatcher.Metadata.RebuildMetadata();

        InitializeFileWatcher(pluginsSource);
    }

    public PluginManager(string pluginsSource)
    {
        var logRepository = new LogRepository();
        _logger = new PluginLoggerService(logRepository);
        _lifecycleManager = new PluginLifecycleManager();

        var loggerFacade = new PluginLoggingFacade(_logger);
        var assemblyHandler = new AssemblyHandler();
        var assemblyLoader = new AssemblyLoader(loggerFacade, pluginsSource);
        var repository = new AssemblyMetadataRepository();
        var pluginExecutor = new PluginExecutor(_lifecycleManager, loggerFacade);
        var dependencyResolver = new PluginDependencyResolver(repository, assemblyLoader, assemblyHandler,
            loggerFacade);

        Initialize(pluginsSource, repository, assemblyLoader, assemblyHandler, pluginExecutor,
            _lifecycleManager, loggerFacade, dependencyResolver);
    }

    internal PluginManager(string pluginsSource, IPluginLifecycleManager lifecycleManager,
        IAssemblyHandler handler, IAssemblyLoader loader, IAssemblyMetadataRepository repository,
        IPluginExecutor executor, ILoggerService logger, IPluginDependencyResolver dependencyResolver)
    {
        _logger = logger;
        _lifecycleManager = lifecycleManager;

        var loggerFacade = new PluginLoggingFacade(_logger);
        Initialize(pluginsSource, repository, loader, handler, executor, lifecycleManager, loggerFacade,
            dependencyResolver);
    }

    private void InitializeFileWatcher(string source)
    {
        _fileWatcher.Path = source;
        _fileWatcher.Filter = "*.dll";

        _fileWatcher.Created += OnAssemblyAdded;
        _fileWatcher.Deleted += OnAssemblyDeleted;
        _fileWatcher.Changed += OnAssemblyUpdated;

        _fileWatcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
        _fileWatcher.Created -= OnAssemblyAdded;
        _fileWatcher.Deleted -= OnAssemblyDeleted;
        _fileWatcher.Changed -= OnAssemblyUpdated;

        _fileWatcher.Dispose();
        GC.SuppressFinalize(this);
    }

    private void OnAssemblyAdded(object obj, FileSystemEventArgs e)
    {
        var name = Path.GetFileNameWithoutExtension(e.Name)!;
        _dispatcher.Metadata.LoadMetadata(name);
        _dispatcher.Unloader.UnloadAssembly(name);
    }

    private void OnAssemblyDeleted(object obj, FileSystemEventArgs e)
        => _dispatcher.Metadata.RemoveMetadata(Path.GetFileNameWithoutExtension(e.Name)!);

    private void OnAssemblyUpdated(object obj, FileSystemEventArgs e)
    {
        var name = Path.GetFileNameWithoutExtension(e.Name)!;
        _dispatcher.Metadata.UpdateMetadata(name);
        _dispatcher.Unloader.UnloadAssembly(name);
    }


    /// <summary>
    /// Launches a standard plugin by its name. 
    /// After execution, the assembly containing the plugin is unloaded if it is no longer in use.
    /// </summary>
    /// <param name="pluginName">The name of the standard plugin to be executed.</param>
    /// <exception cref="PluginNotFoundException">Thrown if the specified plugin is not found.</exception>
    public void ExecutePlugin(string pluginName)
    {
        _dispatcher.Starter.StartPlugin(pluginName);
        _dispatcher.Unloader.UnloadAssemblyByPluginName(pluginName);
    }

    /// <summary>
    /// Runs all standard plugins from the specified assembly. 
    /// After execution, the assembly is unloaded if no references remain.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly containing the standard plugins.</param>
    /// <exception cref="AssemblyNotFoundException">Thrown if the specified assembly is not found.</exception>
    public void ExecutePluginsFromAssembly(string assemblyName)
    {
        _dispatcher.Starter.StartAllPluginsFromAssembly(assemblyName);
        _dispatcher.Unloader.UnloadAssembly(assemblyName);
    }

    /// <summary>
    /// Runs all standard plugins from all detected assemblies in the plugin directory. 
    /// After execution, each assembly is unloaded if it is no longer referenced.
    /// </summary>
    public void ExecuteAllPlugins()
    {
        _dispatcher.Starter.StartAllPlugins();
        _dispatcher.Unloader.UnloadAllAssemblies();
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
        _dispatcher.Starter.StartExtensionPlugin(ref data, pluginName);
        _dispatcher.Unloader.UnloadAssemblyByPluginName(pluginName);
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
            _dispatcher.Starter.SendNetworkPlugin(pluginName, requestData);

        var response = expectResponse ? _dispatcher.Starter.ReceiveNetworkPlugin(pluginName) : null;
        _dispatcher.Unloader.UnloadAssemblyByPluginName(pluginName);

        return response;
    }

    /// <summary>
    /// Retrieves the state of a specific plugin by its name.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose state is to be retrieved.</param>
    /// <returns>The current state of the specified plugin as a string.</returns>
    public PluginInfo GetPluginState(string pluginName)
        => _lifecycleManager.GetPluginState(pluginName);

    /// <summary>
    /// Retrieves the states of all plugins.
    /// </summary>
    /// <returns>An enumerable collection of PluginInfo objects representing the states of all plugins.</returns>
    public IEnumerable<PluginInfo> GetPluginStates()
        => _lifecycleManager.GetPluginStates();

    /// <summary>
    /// Retrieves a list of messages from the logger.
    /// </summary>
    /// <returns>A collection of strings containing logged messages.</returns>
    public IEnumerable<string> GetMessagesFromLogger()
        => _logger.GetLogs();

    

    /// <summary>
    /// Exports logs with filtering, excluding specified log types.
    /// </summary>
    /// <param name="exporter">
    /// An instance implementing the <see cref="ILogExporter"/> interface, used for exporting logs.
    /// </param>
    /// <param name="exceptLogTypes">
    /// A collection of log types to be excluded from the exported log.
    /// </param>
    /// <remarks>
    /// This method delegates the log export task to the logging service while excluding all messages of the specified log types.
    /// It provides flexibility in configuring which messages will be included in the exported log.
    /// </remarks>
    public void ExportFilteredLogs(ILogExporter exporter, IEnumerable<LogType> exceptLogTypes)
        => _logger.ExportLogs(exporter, exceptLogTypes);

    /// <summary>
    /// Exports a user-friendly log, excluding debug (<see cref="LogType.DEBUG"/>) and trace (<see cref="LogType.TRACE"/>) messages.
    /// </summary>
    /// <param name="exporter">
    /// An instance implementing the <see cref="ILogExporter"/> interface, responsible for exporting logs.
    /// </param>
    /// <remarks>
    /// This method uses <see cref="ExportFilteredLogs(ILogExporter, IEnumerable{LogType})"/> to export logs with filtering.
    /// Excluding debug and trace messages ensures that only essential log information is included, reducing noise for end users.
    /// </remarks>
    public void ExportLogs(ILogExporter exporter)
        => ExportFilteredLogs(exporter, [LogType.DEBUG, LogType.TRACE]);

    /// <summary>
    /// Exports the full log using the specified log exporter, excluding TRACE-level messages.
    /// </summary>
    /// <param name="exporter">
    /// An instance implementing the <see cref="ILogExporter"/> interface, responsible for exporting logs to a designated storage (e.g., a file or database).
    /// </param>
    /// <remarks>
    /// This method exports all logs except TRACE-level messages, providing a comprehensive log without low-level trace details.
    /// </remarks>
    public void ExportDebugLogs(ILogExporter exporter)
        => ExportFilteredLogs(exporter, [LogType.TRACE]);

    /// <summary>
    /// Exports the full detailed log, including all log levels such as TRACE.
    /// </summary>
    /// <param name="exporter">
    /// An instance implementing the <see cref="ILogExporter"/> interface, responsible for exporting logs to a designated storage (e.g., a file or database).
    /// </param>
    /// <remarks>
    /// This method exports all log entries without any filtering, including the most detailed TRACE-level logs.
    /// </remarks>
    public void ExportTraceLogs(ILogExporter exporter)
        => _logger.ExportLogs(exporter);
}