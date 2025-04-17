using PluginAPI;

namespace TestAssembly;

public class FaultedNetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkFaultedPlugin";
    public override Version Version => new(1, 0, 0);
    public override string Description => "Faulted plugin";
    public override string Author => "kand1s";

    public Task<string> SendDataAsync(byte[] data)
    {
        throw new Exception();
    }

    public Task<byte[]> ReceiveDataAsync()
    {
        throw new Exception();
    }
}