using PluginAPI;

namespace TestAssembly;

public class NetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Author => "kand1s";
    
    
    public async Task<string> SendDataAsync(byte[] data)
    {
        Console.WriteLine(data);
        await Task.Yield();
        return "";
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        await Task.Yield();
        return [];
    }
}