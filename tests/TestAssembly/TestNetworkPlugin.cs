using PluginAPI;

namespace PluginManagerTests.Test_Plugins;

public class TestNetworkPlugin : INetworkPlugin
{
    public string Name { get; } = string.Empty;
    public Version Version { get; } = new(1, 0, 0);
    public string Description { get; } = string.Empty;
    public string Author { get; } = string.Empty;
    
    
    public bool SendCalled { get; private set; } = false;
    public bool ReceiveCalled { get; private set; } = false;
    
    
    public Task SendDataAsync(byte[] data)
    {
        SendCalled = true;
        return Task.CompletedTask;
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        ReceiveCalled = true;
        await Task.Yield();
        return [];
    }
}