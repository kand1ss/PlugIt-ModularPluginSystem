namespace PluginAPI;

public interface INetworkPlugin : IPlugin
{
    Task<string> SendDataAsync(byte[] data);
    Task<byte[]> ReceiveDataAsync();
}