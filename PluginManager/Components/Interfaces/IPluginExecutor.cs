using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginExecutor
{
    void Execute(IPlugin plugin);
    void Execute(IEnumerable<IPlugin> plugins);
    
    void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension);
    void ExecuteExtensionPlugins<T>(ref T data, IEnumerable<IExtensionPlugin<T>> extensions);
    
    byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin);
    void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin);
}