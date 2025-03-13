using ModularPluginAPI.Components.Lifecycle;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(IPluginLifecycleManager lifecycleManager) : IPluginExecutor
{
    public void Execute(IPlugin plugin)
    {
        if (plugin is IInitialisablePlugin initPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Initializing);
            initPlugin.Initialize();
        }

        if (plugin is IExecutablePlugin executablePlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
            executablePlugin.Execute();
        }

        if (plugin is IFinalisablePlugin finalPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Finalizing);
            finalPlugin.FinalizePlugin();
        }
    }

    public void Execute(IEnumerable<IPlugin> plugins)
        => plugins.ToList().ForEach(Execute);


    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension)
    {
        lifecycleManager.SetPluginState(extension.Name, PluginState.Running);
        extension.Expand(ref data);
    }

    public void ExecuteExtensionPlugins<T>(ref T data, IEnumerable<IExtensionPlugin<T>> extensions)
    {
        foreach (var extension in extensions)
            ExecuteExtensionPlugin(ref data, extension);
    }

    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
    {
        lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
        return plugin.ReceiveData();
    }

    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
    {
        lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
        plugin.SendData(data);
    }
}