using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginExecutor 
{
    void Execute(IPluginData plugin);
    void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> plugin);
    Task<byte[]> ExecuteNetworkPluginReceiveAsync(INetworkPlugin plugin);
    Task ExecuteNetworkPluginSendAsync(byte[] data, INetworkPlugin plugin);
    Task<byte[]> ExecuteFilePluginReadAsync(IFilePlugin plugin);
    Task ExecuteFilePluginWriteAsync(byte[] data, IFilePlugin plugin);
}