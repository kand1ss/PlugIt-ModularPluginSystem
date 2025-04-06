namespace PluginAPI;

public interface INetworkPlugin : IPlugin
{
    void SendData(byte[] data);
    byte[] ReceiveData();
}