using PluginAPI;

namespace TestAssembly;

public class NetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Network plugin";
    public override string Author => "kand1s";
    
    
    public Task SendDataAsync(byte[] data)
    {
        Console.WriteLine(data);
        return Task.CompletedTask;
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        await Task.Yield();
        return [];
    }
}