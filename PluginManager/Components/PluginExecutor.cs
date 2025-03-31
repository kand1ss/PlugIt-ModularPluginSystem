using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(IPluginTracker tracker, PluginLoggingFacade logger) : IPluginExecutor
{
    private void TryInitializePlugin(IPlugin plugin)
    {
        if (plugin is IInitialisablePlugin initPlugin)
        {
            tracker.SetPluginState(plugin.Name, PluginState.Initializing);
            logger.PluginInitialized(plugin.Name, plugin.Version);
            
            initPlugin.Initialize();
        }
    }
    private void TryExecutePlugin(IPlugin plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            tracker.SetPluginState(plugin.Name, PluginState.Running);
            logger.PluginExecuted(plugin.Name, plugin.Version);
            
            executablePlugin.Execute();
        }
    }
    private void TryFinalizePlugin(IPlugin plugin)
    {
        if (plugin is IFinalisablePlugin finalPlugin)
        {
            tracker.SetPluginState(plugin.Name, PluginState.Finalizing);
            logger.PluginFinalized(plugin.Name, plugin.Version);
            
            finalPlugin.FinalizePlugin();
        }
    }
    private void ExecuteAction(Action<IPlugin> action, IPlugin plugin)
    {
        TryInitializePlugin(plugin);
        action(plugin);
        TryFinalizePlugin(plugin);
    }
    
    
    public void Execute(IPlugin plugin)
        => ExecuteAction(TryExecutePlugin, plugin);

    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension)
    {
        TryInitializePlugin(extension);
        
        tracker.SetPluginState(extension.Name, PluginState.Running);
        logger.ExtensionPluginExecuting(extension.Name, extension.Version);
        
        extension.Expand(ref data);
        
        TryFinalizePlugin(extension);
    }
    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
    {
        TryInitializePlugin(plugin);
        
        tracker.SetPluginState(plugin.Name, PluginState.Running);
        logger.NetworkPluginExecuting(plugin.Name, plugin.Version, false);
        
        var result = plugin.ReceiveData();
        
        TryFinalizePlugin(plugin);
        return result;
    }
    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => ExecuteAction(addon =>
        {
            tracker.SetPluginState(plugin.Name, PluginState.Running);
            logger.NetworkPluginExecuting(plugin.Name, plugin.Version, true);
            
            plugin.SendData(data);
        }, plugin);
}