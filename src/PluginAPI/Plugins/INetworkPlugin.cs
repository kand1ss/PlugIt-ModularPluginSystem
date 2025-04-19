namespace PluginAPI;

public interface INetworkPlugin : IConfigurablePlugin
{
    Task<string> SendDataAsync(byte[] data);
    Task<byte[]> ReceiveDataAsync();
}