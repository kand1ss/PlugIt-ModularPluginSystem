using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin2 : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Network plugin";
    public override string Author => "kand1s";


    public Task SendDataAsync(byte[] data)
    {
        Console.WriteLine("Network plugin sent");
        return Task.CompletedTask;
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        Console.WriteLine("Network plugin received");
        await Task.Yield();
        return [];
    }
}