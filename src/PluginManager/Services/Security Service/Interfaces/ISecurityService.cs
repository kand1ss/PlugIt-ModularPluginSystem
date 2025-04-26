
using ModularPluginAPI.Components;

namespace ModularPluginAPI.Services.Interfaces;

/// <summary>
/// Provides security-related services for managing and validating plugin assemblies, namespaces,
/// and resource permissions in a controlled environment.
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Represents the settings used to configure security-related features for plugin management.
    /// Provides access to specific settings for network and file system operations.
    /// </summary>
    public ISecuritySettings Settings { get; }


    /// <summary>
    /// Imports a JSON configuration file to configure security settings.
    /// The configuration file is expected to define permissions, blocked namespaces, and other security-related settings.
    /// </summary>
    /// <param name="configurationPath">The full file path to the JSON configuration file to import.</param>
    /// <returns>
    /// <c>true</c> if the configuration was successfully imported and applied;
    /// otherwise, <c>false</c> if the process failed, such as due to an invalid file path or format issues.
    /// </returns>
    bool ImportJsonConfiguration(string configurationPath);

    /// <summary>
    /// Checks the specified assembly (.dll file) for the usage of blocked namespaces.
    /// This static analysis is performed only if security checks are enabled in <see cref="PluginManagerSettings"/>.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly (.dll) to check.</param>
    /// <returns>
    /// <c>true</c> if the assembly is considered safe and does not use any blocked namespaces;
    /// otherwise, <c>false</c>.
    /// </returns>
    bool CheckAssemblySafety(string assemblyPath);

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

    /// <summary>
    /// Grants permission to access a specific file system path for all plugins.
    /// This permission allows plugins to interact with the specified path based on the provided read, write, and recursive permissions.
    /// </summary>
    /// <param name="fullPath">The full file system path to grant access to (e.g., "D:/Plugins/Data").</param>
    /// <param name="canRead">Indicates whether reading the specified path is permitted. Defaults to <c>true</c>.</param>
    /// <param name="canWrite">Indicates whether writing to the specified path is permitted. Defaults to <c>true</c>.</param>
    /// <param name="recursive">Indicates whether the permission applies to subdirectories and files within the specified path. Defaults to <c>false</c>.</param>
    void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = true);

    /// <summary>
    /// Grants permission to access a specific network resource (URL) for all plugins.
    /// This permission allows plugins to perform network operations on the given URL if it is declared in their configuration.
    /// </summary>
    /// <param name="url">The URL to allow access to (e.g., "https://api.example.com"). Must be a valid HTTP or HTTPS URL.</param>
    /// <param name="canRead">Determines whether read access is permitted for the specified network resource. Default is <c>true</c>.</param>
    /// <param name="canWrite">Determines whether write access is permitted for the specified network resource. Default is <c>true</c>.</param>
    void AddNetworkPermission(string url, bool canRead = true, bool canWrite = true);
}