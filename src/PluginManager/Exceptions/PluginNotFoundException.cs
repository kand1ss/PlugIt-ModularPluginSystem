namespace ModularPluginAPI.Exceptions;

public class PluginNotFoundException : Exception
{
    public PluginNotFoundException(string pluginName) : base($"Plugin '{pluginName}' could not be found.")
    {
    }

    public PluginNotFoundException(string pluginName, string assemblyName) 
        : base($"Plugin '{pluginName}' from assembly '{assemblyName} could not be found.")
    {
    }
}