namespace PluginInfrastructure.Network_Service;

public class NetworkServiceSettings
{
    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public int MaxResponseSize { get; init; } = 100 * 1024 * 1024;
    public int MaxRequestRetriesCount { get; init; } = 0;
    public int MaxRedirectionsCount { get; init; } = 3;
}