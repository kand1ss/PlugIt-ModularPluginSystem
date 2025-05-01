using PluginAPI;

namespace PluginManagerTests.Test_Plugins;

public class TestNetworkPlugin : NetworkPluginBase
{
    public override string Name { get; } = string.Empty;
    public override Version Version { get; } = new(1, 0, 0);
    public override string Author { get; } = string.Empty;
    
    
    public bool SendCalled { get; private set; } = false;
    public bool ReceiveCalled { get; private set; } = false;
    
    
    public override async Task<string> SendDataAsync(byte[] data)
    {
        SendCalled = true;
        await Task.Yield();
        return "";
    }

    public override async Task<byte[]> ReceiveDataAsync()
    {
        ReceiveCalled = true;
        await Task.Yield();
        return [];
    }
}