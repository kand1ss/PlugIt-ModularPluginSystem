using PluginAPI.Services.interfaces;

namespace PluginAPI.Stubs;

public class NetworkServiceStub : IPluginNetworkService
{
    public Task<byte[]> GetAsync(string url)
        => throw new InvalidOperationException("Network service is not implemented");

    public Task<string> PostAsync(string url, HttpContent content)
        => throw new InvalidOperationException("Network service is not implemented");
}