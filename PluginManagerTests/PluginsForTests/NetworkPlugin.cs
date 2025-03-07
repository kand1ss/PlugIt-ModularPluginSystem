using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin : INetworkPlugin
{
    public string Name => "NetworkPlugin";
    public string Version => "1.0.0.0";
    public string Description => "Network plugin";
    
    public async Task SendDataAsync(byte[] data)
    {
        await Task.Delay(10);
        Console.WriteLine("Network plugin sent");
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        await Task.Delay(10);
        Console.WriteLine("Network plugin received");
        return [];
    }
}