using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin : INetworkPlugin
{
    public string Name => "NetworkPlugin";
    public Version Version => new(1, 0, 0);
    public string Description => "Network plugin";
    public string Author => "kand1s";
    
    
    public void SendData(byte[] data)
    {
        Console.WriteLine("Network plugin sent");
    }

    public byte[] ReceiveData()
    {
        Console.WriteLine("Network plugin received");
        return [];
    }
}