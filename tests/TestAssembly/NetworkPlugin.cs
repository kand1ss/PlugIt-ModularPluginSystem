using PluginAPI;

namespace TestAssembly;

public class NetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Network plugin";
    public override string Author => "kand1s";
    
    
    public void SendData(byte[] data)
    {
        Console.WriteLine(data);
    }

    public byte[] ReceiveData()
    {
        return [];
    }
}