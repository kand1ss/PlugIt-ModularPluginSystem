using ModularPluginAPI.Models;

namespace ModularPluginAPI.Services.Interfaces;

/// <summary>
/// Represents a service that verifies the safety and security permissions of plugins.
/// </summary>
public interface IPluginPermissionSecurityService
{
    /// <summary>
    /// Verifies the safety and security permissions of a specified plugin.
    /// </summary>
    /// <param name="pluginMetadata">The metadata containing information about the plugin, including its configuration, permissions, and other details.</param>
    /// <returns><c>true</c> if the plugin's permissions are deemed safe; otherwise, <c>false</c>.</returns>
    bool CheckSafety(PluginMetadata pluginMetadata);
}