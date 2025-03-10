using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin2 : INetworkPlugin
{
    public string Name => "NetworkPlugin2";
    public Version Version => new(1, 0, 0);
    public string Description => "Network plugin";
    
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