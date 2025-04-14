namespace PluginAPI;

// TODO - обновить API
public interface INetworkPlugin : IPlugin
{
    void SendData(byte[] data);
    byte[] ReceiveData();
}