namespace ModularPluginAPI.Exceptions;

public class PluginNotFoundException(string pluginName) : Exception($"Plugin '{pluginName}' could not be found.");