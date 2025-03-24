using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginExecutor
{
    ExecutionResult Execute(IPlugin plugin);
    ExecutionResult ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension);
    ExecutionResult ExecuteNetworkPluginReceive(INetworkPlugin plugin, out byte[] response);
    ExecutionResult ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin);
}