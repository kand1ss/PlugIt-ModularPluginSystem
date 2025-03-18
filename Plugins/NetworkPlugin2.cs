using PluginAPI;

namespace PluginManagerTests.PluginsForTests;

public class NetworkPlugin2 : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin2";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Network plugin";
    public override string Author => "kand1s";
    
    
    public override void Initialize()
    {
    }

    public override void FinalizePlugin()
    {
    }


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