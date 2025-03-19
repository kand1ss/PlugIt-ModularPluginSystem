namespace ModularPluginAPI.Components.Logger;

public class PluginLoggerLayer(ILoggerService logger)
{
    public void MetadataAdded(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Metadata for assembly '{assemblyName} v{assemblyVersion}' added.");
    }
    public void MetadataRemoved(string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Metadata for assembly '{assemblyName} v{assemblyVersion}' removed.");
    }
    
    
    
    
    public void PluginLoaded(string pluginName, string assemblyName, Version assemblyVersion)
    {
        logger.Log(LogSender.PluginManager, LogType.INFO, 
            $"Plugin '{pluginName}' from assembly '{assemblyName} v{assemblyVersion}' loaded.");
    }
}