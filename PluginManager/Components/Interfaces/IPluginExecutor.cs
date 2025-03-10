using PluginAPI;

namespace ModularPluginAPI.Components;

public interface IPluginExecutor
{
    void Execute(IPlugin plugin);
    void Execute(IEnumerable<IPlugin> plugins);
    void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension);
}