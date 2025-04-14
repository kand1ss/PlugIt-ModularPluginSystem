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

    /// <summary>
    /// Adds a namespace to the list of blocked namespaces, preventing its usage in plugin assemblies.
    /// The namespace must be valid and contain only letters and dots.
    /// </summary>
    /// <param name="namespaceName">The name of the namespace to block.</param>
    /// <returns>
    /// <c>true</c> if the namespace was successfully added;
    /// <c>false</c> if the name was invalid or already exists in the list.
    /// </returns>
    bool AddBlockedNamespace(string namespaceName);

    /// <summary>
    /// Removes a namespace from the list of blocked namespaces.
    /// The namespace must be valid and already exist in the list.
    /// </summary>
    /// <param name="namespaceName">The name of the namespace to remove.</param>
    /// <returns>
    /// <c>true</c> if the namespace was successfully removed;
    /// <c>false</c> if the name was invalid or not found in the list.
    /// </returns>
    bool RemoveBlockedNamespace(string namespaceName);
}
