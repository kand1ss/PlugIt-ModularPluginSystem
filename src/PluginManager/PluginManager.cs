using ModularPluginAPI.Components;
using ModularPluginAPI.Components.AssemblyWatcher;
using ModularPluginAPI.Components.AssemblyWatcher.Interfaces;
using ModularPluginAPI.Components.ErrorRegistry;
using ModularPluginAPI.Components.ErrorRegistry.Interfaces;
using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Logger.Components;
using ModularPluginAPI.Components.Logger.Interfaces;
using ModularPluginAPI.Components.Profiler;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Services.Interfaces;

namespace ModularPluginAPI;

public class PluginManager
{
    private readonly PluginDispatcher _dispatcher;
    private readonly ILoggerService _logger;
    private readonly IPluginPerformanceProfiler _profiler;

    /// <summary>
    /// Provides access to the plugin tracking API, allowing integration with custom components 
    /// that respond to plugin registration, removal, and state changes.
    /// </summary>
    /// <remarks>
    /// External components can subscribe to plugin events through this tracker, enabling 
    /// real-time monitoring and custom reactions to plugin lifecycle changes.
    /// </remarks>
    public IPluginTrackerPublic Tracker => _tracker;
    private readonly IPluginTracker _tracker;
    
    /// <summary>
    /// Gets the error registry that stores information about occurred errors.
    /// </summary>
    /// <remarks>
    /// The error registry will save errors only if it is enabled (by default, it is always enabled).
    /// </remarks>
    public IPluginErrorRegistry ErrorRegistry { get; }
    
    
    /// <summary>
    /// Gets the security service for the plugin manager.
    /// This component is responsible for handling the security of plugins, including performing static analysis and enforcing security policies.
    /// The security service will only be active if the security feature has not been disabled in <see cref="PluginManagerSettings"/>.
    /// If <c>EnableSecurityService</c> is set to <c>false</c> in <see cref="PluginManagerSettings"/>, this service will not be available.
    /// </summary>
    public IAssemblySecurityService Security { get; }
    

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginManager"/> class with the specified settings.
    /// </summary>
    /// <param name="settings">
    /// An optional instance of <see cref="PluginManagerSettings"/> that allows customization of the plugin manager's behavior.
    /// If no settings are provided, default settings will be used.
    /// </param>
    /// <remarks>
    /// If no settings are specified, the plugin manager will use default settings, which include predefined behaviors like enabling performance profiling.
    /// 
    /// Example setting: You can disable performance profiling by setting the <see cref="PluginManagerSettings.EnableProfiling"/> property to <c>false</c> in the <paramref name="settings"/>.
    /// </remarks>
    public PluginManager(PluginManagerSettings? settings = null)
    {
        settings ??= new PluginManagerSettings();
        
        var logRepository = new LogRepository();
        _logger = new PluginLoggerService(logRepository);
        var loggerFacade = new PluginLoggingFacade(_logger);
        
        var pluginTracker = new PluginTracker(loggerFacade);
        _tracker = pluginTracker;
        
        var assemblyHandler = new AssemblyHandler();
        var assemblyLoader = new AssemblyLoader(loggerFacade);
        var securityService = new AssemblySecurityService();
        Security = securityService;
        var repository = new AssemblyMetadataRepository();
        repository.AddObserver(pluginTracker);
        if (settings.EnableSecurity)
            repository.AddObserver(securityService);
        
        var pluginExecutor = new PluginExecutor(loggerFacade);
        pluginExecutor.AddObserver(pluginTracker);
        
        var pluginProfiler = new PluginPerformanceProfiler();
        _profiler = pluginProfiler;
        if (settings.EnableProfiling)
            pluginExecutor.AddObserver(pluginProfiler);

        var errorRegistry = new PluginErrorRegistry();
        ErrorRegistry = errorRegistry;
        var errorHandledPluginExecutor = new ErrorHandlingPluginExecutor(pluginExecutor, _tracker, loggerFacade);
        if (settings.EnableErrorRegistry)
            errorHandledPluginExecutor.AddObserver(errorRegistry);
        errorHandledPluginExecutor.AddObserver(pluginTracker);
        errorHandledPluginExecutor.AddObserver(pluginProfiler);

        var metadataService = new PluginMetadataService(repository);
        var loaderService = new PluginLoaderService(metadataService, assemblyLoader, assemblyHandler);
        var dependencyResolver = new DependencyResolverService(loaderService, metadataService, loggerFacade);

        var assemblyWatcher = new AssemblyWatcher();
        _dispatcher = new(repository, assemblyLoader, assemblyHandler, errorHandledPluginExecutor, 
            _tracker,loggerFacade, loaderService, metadataService, dependencyResolver, assemblyWatcher);
    }

    internal PluginManager(IPluginTracker tracker,
        IAssemblyHandler handler, IAssemblyLoader loader, IAssemblyMetadataRepository repository,
        IPluginExecutor executor, ILoggerService logger, IPluginLoaderService loaderService, 
        IPluginMetadataService metadataService, IDependencyResolverService dependencyResolver, IAssemblyWatcher watcher,
        IPluginPerformanceProfiler performanceProfiler, IPluginErrorRegistry errorRegistry, IAssemblySecurityService security)
    {
        _logger = logger;
        _tracker = tracker;
        ErrorRegistry = errorRegistry;
        _profiler = performanceProfiler;
        Security = security;

        var loggerFacade = new PluginLoggingFacade(_logger);
        _dispatcher = new(repository, loader, handler, executor, _tracker, loggerFacade, loaderService, 
            metadataService, dependencyResolver, watcher);
    }

    /// <summary>
    /// Registers the assembly metadata from the specified assembly file.
    /// </summary>
    /// <param name="assemblyPath">
    /// The full path of the assembly whose metadata should be loaded and registered.
    /// </param>
    /// <remarks>
    /// This method loads metadata for the specified assembly into the manager's repository,
    /// thereby making the contained plugins discoverable. Once the metadata is loaded,
    /// the corresponding assembly is unloaded if it is no longer in use.
    /// </remarks>
    public void RegisterAssembly(string assemblyPath)
    {
        _dispatcher.RegisterAssembly(assemblyPath);
        _dispatcher.Metadata.LoadMetadata(assemblyPath);
        _dispatcher.Unloader.UnloadAssembly(assemblyPath);
    }

    /// <summary>
    /// Registers the assembly metadata for all assemblies found in the specified directory.
    /// </summary>
    /// <param name="directoryPath">
    /// The path to the directory containing assemblies to load metadata from.
    /// </param>
    /// <remarks>
    /// This method loads metadata for each assembly found in the given directory,
    /// enabling the manager to discover the plugins contained within those assemblies.
    /// After loading the metadata, any assemblies that are not in use will be unloaded.
    /// </remarks>
    public void RegisterAssembliesFromDirectory(string directoryPath)
    {
        _dispatcher.RegisterAssembliesFromDirectory(directoryPath);
        _dispatcher.Metadata.LoadMetadataFromDirectory(directoryPath);
        _dispatcher.Unloader.UnloadAssembliesFromDirectory(directoryPath);
    }

    /// <summary>
    /// Unregisters the assembly metadata for the specified assembly.
    /// </summary>
    /// <param name="assemblyPath">
    /// The full path of the assembly whose metadata should be removed from the repository.
    /// </param>
    /// <remarks>
    /// This method removes the metadata for the specified assembly from the manager's repository,
    /// effectively marking the plugins contained in that assembly as no longer available.
    /// </remarks>
    public void UnregisterAssembly(string assemblyPath)
    {
        _dispatcher.UnregisterAssembly(assemblyPath);
        _dispatcher.Metadata.RemoveMetadata(assemblyPath);
    }

    /// <summary>
    /// Unregisters the assembly metadata for all assemblies in the specified directory.
    /// </summary>
    /// <param name="directoryPath">
    /// The path to the directory containing assemblies whose metadata should be removed.
    /// </param>
    /// <remarks>
    /// This method removes the metadata for all assemblies found in the specified directory from the manager's repository,
    /// effectively making the plugins contained in those assemblies undiscoverable.
    /// </remarks>
    public void UnregisterAssembliesFromDirectory(string directoryPath)
    {
        _dispatcher.UnregisterAssembliesFromDirectory(directoryPath);
        _dispatcher.Metadata.RemoveMetadataFromDirectory(directoryPath);
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

    /// <summary>
    /// Exports the collected profiler logs using the specified log exporter.
    /// </summary>
    /// <param name="exporter">
    /// An instance of <see cref="ILogExporter"/> that handles the export of the profiler logs.
    /// </param>
    /// <remarks>
    /// This method delegates the export process to the underlying profiler logger,
    /// which formats and sends the profiling data to the given exporter.
    /// </remarks>
    public void ExportProfilerLogs(ILogExporter exporter)
        => _profiler.ExportProfilerLogs(exporter);
}