namespace ModularPluginAPI.Components.Logger;

public class PluginLoggingFacade(ILoggerService logger)
{
    public void LogError(string errorMessage)
    {
        logger.Log(LogSender.PluginManager, LogType.ERROR, errorMessage);
    }
    
    public void AssemblyLoaded(string assemblyName)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, $"Assembly '{assemblyName}' loaded.");
    }
    public void AssemblyUnloaded(string assemblyName)
    {
        logger.Log(LogSender.PluginManager, LogType.TRACE, $"Assembly '{assemblyName}' unloaded.");
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

    public void DependencyLoaded(string dependencyName, Version dependencyVersion, string assemblyName,
        Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.DEBUG, 
            $"Dependency '{dependencyName} v{dependencyVersion}' from assembly '{assemblyName} v{assemblyVersion}' loaded.");
    }


    public void DirectoryChanged(string directory)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, $"Plugin directory changed to: '{directory}'.");
    }
}