using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin2 : NetworkPluginBase
{
    public override string Name => "NetworkPlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Network plugin";
    public override string Author => "kand1s";


    public override async Task<string> SendDataAsync(byte[] data)
    {
        Console.WriteLine("Network plugin sent");
        await Task.Yield();
        return "";
    }

    public override async Task<byte[]> ReceiveDataAsync()
    {
        Console.WriteLine("Network plugin received");
        return await NetworkService.GetAsync("https://httpbin.org/get");
    }
}