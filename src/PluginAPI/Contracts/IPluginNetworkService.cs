namespace PluginAPI.Services.interfaces;

/// <summary>
/// Defines methods for interacting with network resources securely within a plugin environment.
/// </summary>
public interface IPluginNetworkService
{
    /// <summary>
    /// Sends an asynchronous GET request to the specified URL and retrieves the response as a byte array.
    /// </summary>
    /// <param name="url">The URL to which the GET request is sent. It must be a non-empty, valid URL.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the response data as a byte array.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the provided URL is null, empty, or invalid.</exception>
    /// <exception cref="SecurityException">Thrown when the URL is not allowed or proper permissions are not configured.</exception>
    Task<byte[]> GetAsync(string url);

    /// <summary>
    /// Sends an asynchronous POST request to the specified URL with the provided HTTP content
    /// and retrieves the response as a string.
    /// </summary>
    /// <param name="url">The URL to which the POST request is sent. It must be a non-empty, valid URL.</param>
    /// <param name="content">An instance of <see cref="HttpContent"/> that represents the data to send in the POST request body.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the response data as a string.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the provided URL is null, empty, or invalid.</exception>
    /// <exception cref="SecurityException">
    /// Thrown when the URL is not allowed by the network permissions or if POST permissions are not properly configured.
    /// </exception>
    Task<string> PostAsync(string url, HttpContent content);
}