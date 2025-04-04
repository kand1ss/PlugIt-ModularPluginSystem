using ModularPluginAPI.Components.Interfaces;
using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginExecutor : IObservablePluginExecutor
{
    void Execute(IPlugin plugin);
    void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> plugin);
    byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin);
    void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin);
}