using PluginAPI.Services.interfaces;
using PluginAPI.Stubs;

namespace PluginAPI;

public abstract class NetworkPluginBase : PluginBase, INetworkPlugin
{
    private bool _isServiceInjected = false;
    protected IPluginNetworkService NetworkService { get; private set; } = new NetworkServiceStub();
    
    public void InjectService(IPluginNetworkService service)
        => NetworkService = InjectService<IPluginNetworkService>.TryInject(service, ref _isServiceInjected);
    
    public abstract Task<string> SendDataAsync(byte[] data);
    public abstract Task<byte[]> ReceiveDataAsync();
}