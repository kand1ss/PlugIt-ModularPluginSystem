namespace ModularPluginAPI.Services.Interfaces;

/// <summary>
/// Provides functionality for static security analysis of plugin assemblies.
/// </summary>
public interface IAssemblySecurityService
{
    /// <summary>
    /// Checks the specified assembly (.dll file) for the usage of blocked namespaces.
    /// This static analysis is performed only if security checks are enabled in <see cref="PluginManagerSettings"/>.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly (.dll) to check.</param>
    /// <returns>
    /// <c>true</c> if the assembly is considered safe and does not use any blocked namespaces;
    /// otherwise, <c>false</c>.
    /// </returns>
    bool CheckSafety(string assemblyPath);
}
