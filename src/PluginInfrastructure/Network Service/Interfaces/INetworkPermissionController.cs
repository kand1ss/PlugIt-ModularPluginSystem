using PluginAPI.Models.Permissions;

namespace PluginInfrastructure.Network_Service.Interfaces;

/// <summary>
/// Interface for managing network permission rules and maintaining a collection of allowed URLs with associated permissions.
/// </summary>
public interface INetworkPermissionController
{
    /// <summary>
    /// Adds an allowed URL to the collection with its associated permission settings.
    /// The URL is normalized before being added to ensure consistency.
    /// </summary>
    /// <param name="url">The URL to allow. Must be a valid and absolute HTTP or HTTPS URL.</param>
    /// <param name="permission">The permission settings to associate with the URL. Defaults to enabling GET and POST requests if not provided.</param>
    /// <exception cref="ArgumentException">Thrown if the URL is invalid or does not use the HTTP/HTTPS scheme.</exception>
    void AddAllowedUrl(string url, NetworkPermission permission);

    /// <summary>
    /// Retrieves a collection of allowed URLs along with their associated network permissions.
    /// </summary>
    /// <returns>
    /// A dictionary where the keys represent allowed URLs as strings and the values are the corresponding
    /// <see cref="NetworkPermission"/> objects defining the permissions for each URL.
    /// </returns>
    IDictionary<string, NetworkPermission> GetAllowedUrls();
}