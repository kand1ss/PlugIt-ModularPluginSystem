namespace PluginInfrastructure.Network_Service;

/// <summary>
/// Represents configuration settings for network service operations.
/// These settings determine how network requests are handled, including their timeout,
/// maximum response size, retries, and redirections.
/// </summary>
public class NetworkServiceSettings
{
    /// <summary>
    /// Gets or sets the maximum amount of time to wait for a network request to complete.
    /// This property defines the timeout duration for HTTP requests performed by the network service.
    /// If a request exceeds this duration, the operation will be aborted and an exception will be thrown.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(10);

    
    private long _maxResponseSizeBytes = 50 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the maximum allowable response size in megabytes for network requests.
    /// This property determines the upper size limit of response data that the system will process.
    /// Any response exceeding this size will result in an error or truncation.
    /// </summary>
    public long MaxResponseSizeMb
    {
        get => _maxResponseSizeBytes / (1024 * 1024);
        set => _maxResponseSizeBytes = value * 1024 * 1024;
    }

    /// <summary>
    /// Gets the maximum size, in bytes, of the response that can be processed by the network service.
    /// This property determines the upper limit for the size of data that will be accepted from a network response.
    /// Responses exceeding this size will either be truncated or rejected, depending on the implementation.
    /// </summary>
    public long MaxResponseSizeBytes => _maxResponseSizeBytes;


    /// <summary>
    /// Specifies the maximum number of retry attempts allowed for a network request in case of failure.
    /// A value of 0 indicates that no retries will be attempted.
    /// This property is used to manage transient failures, such as temporary network issues or server overload.
    /// </summary>
    public int MaxRequestRetriesCount { get; set; } = 0;

    /// <summary>
    /// Gets or initializes the maximum number of redirections
    /// allowed during an HTTP request. This setting determines
    /// how many times an HTTP client will automatically follow
    /// redirection responses (such as 3xx status codes).
    /// </summary>
    public int MaxRedirectionsCount { get; set; } = 3;
}