using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor : IPluginExecutor
{
    public void Execute(IPlugin plugin)
    {
        if(plugin is IInitialisablePlugin initPlugin)
            initPlugin.Initialize();
        if(plugin is IExecutablePlugin executablePlugin)
            executablePlugin.Execute();
        if(plugin is IFinalisablePlugin finalPlugin)
            finalPlugin.FinalizePlugin();
    }
    public void Execute(IEnumerable<IPlugin> plugins)
        => plugins.ToList().ForEach(Execute);

    
    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension)
        => extension.Expand(ref data);
    public void ExecuteExtensionPlugins<T>(ref T data, IEnumerable<IExtensionPlugin<T>> extensions)
        => extensions.ToList().ForEach(Execute);


    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
        => plugin.ReceiveData();
    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => plugin.SendData(data);
}