using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Lifecycle.Observer;

namespace ModularPluginAPI.Components.Logger;

public class PluginLoggingFacade(ILoggerService logger)
{
    public void AssemblyLoaded(string assemblyName)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, $"(AssemblyLoader) - Assembly '{assemblyName}' loaded.");
    }
    public void AssemblyUnloaded(string assemblyName)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, $"(AssemblyLoader) - Assembly '{assemblyName}' unloaded.");
    }
    
    public void MetadataAdded(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, 
            $"Metadata for assembly '{assemblyName} v{assemblyVersion}' created.");
    }
    public void MetadataRemoved(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, 
            $"Metadata for assembly '{assemblyName} v{assemblyVersion}' removed.");
    }


    public void PluginLoaded(string pluginName, string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Plugin '{pluginName}' from assembly '{assemblyName} v{assemblyVersion}' loaded.");
    }
    public void PluginInitialized(string pluginName, Version pluginVersion)
    {
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Initializing plugin '{pluginName} v{pluginVersion}'.");
    }
    public void PluginExecuted(string pluginName, Version pluginVersion)
    {
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Executing plugin '{pluginName} v{pluginVersion}'.");
    }
    public void ExtensionPluginExecuting(string pluginName, Version pluginVersion)
    {
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Executing extension plugin '{pluginName} v{pluginVersion}'.");
    }
    public void NetworkPluginExecuting(string pluginName, Version pluginVersion, bool isSendMode)
    {
        string message = $"Executing network plugin '{pluginName} v{pluginVersion}' | Mode: ";

        if (isSendMode)
            message += "Send.";
        else
            message += "Receive.";
        
        logger.Log(LogSender.Plugin, LogType.INFO, message);
    }
    public void PluginFinalized(string pluginName, Version pluginVersion)
    {
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Finalizing plugin '{pluginName} v{pluginVersion}'.");
    }

    public void PluginExecutionCompleted(string pluginName, Version pluginVersion)
    {
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Plugin execution completed: '{pluginName} v{pluginVersion}'.");
    }

    public void PluginStateChanged(string pluginName, PluginState pluginState)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, 
            $"(PluginTracker) - Plugin '{pluginName}' state changed: '{pluginState}'");
    }

    public void PluginFaulted(string pluginName, string errorMessage)
    {
        logger.Log(LogSender.Plugin, LogType.INFO,
            $"Plugin '{pluginName}' faulted. Message: '{errorMessage}'.");
    }

    public void DependencyLoaded(string dependencyName, Version dependencyVersion, string assemblyName,
        Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, 
            $"Dependency '{dependencyName} v{dependencyVersion}' from assembly '{assemblyName} v{assemblyVersion}' loaded.");
    }

    public void TrackerComponentRegistered(IPluginTrackerObserver component)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, $"(PluginTracker) - Custom component '{component.GetType().Name}' registered.");
    }
    public void TrackerComponentRemoved(IPluginTrackerObserver component)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, $"(PluginTracker) - Custom component '{component.GetType().Name}' removed.");
    }

    public void SecurityCheckPassed(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, 
            $"(Security) - Assembly: '{assemblyName} v{assemblyVersion}' passed security check.");
    }

    public void SecurityCheckFailed(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"(Security) - Assembly: '{assemblyName} v{assemblyVersion}' failed security check.");
    }
}