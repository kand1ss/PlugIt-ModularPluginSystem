namespace PluginAPI;

public interface INetworkPlugin : IPlugin
{
    Task SendDataAsync(byte[] data);
    Task<byte[]> ReceiveDataAsync();
}