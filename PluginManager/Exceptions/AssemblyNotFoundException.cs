namespace ModularPluginAPI.Exceptions;

public class AssemblyNotFoundException(string assemblyName)
    : Exception($"Assembly '{assemblyName}' could not be found");