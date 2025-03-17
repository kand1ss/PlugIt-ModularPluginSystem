namespace ModularPluginAPI.Exceptions;

public class ResolvingDependencyException(string dependencyName) : Exception($"Cannot resolve dependency {dependencyName}");