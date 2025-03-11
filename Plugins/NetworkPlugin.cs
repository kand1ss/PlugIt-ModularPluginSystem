using PluginAPI;

namespace Plugins;

public class NetworkPlugin : INetworkPlugin
{
    public string Name => "NetworkPlugin";
    public Version Version => new(1, 0, 1);
    public string Description => "Console plugin";
    public string Author => "kand2s";
    
    public void SendData(byte[] data)
    {
        Console.WriteLine($"Sending data: {data.Length} bytes");
    }

    public byte[] ReceiveData()
    {
        Console.WriteLine("Receiving data");
        return [];
    }
}