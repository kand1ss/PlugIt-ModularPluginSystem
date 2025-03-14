using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(IPluginLifecycleManager lifecycleManager, ILoggerService logger) : IPluginExecutor
{
    private void TryInitializePlugin(IPlugin plugin)
    {
        if (plugin is IInitialisablePlugin initPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Initializing);
            logger.Log(LogSender.Plugin, LogType.INFO, 
                $"Initializing plugin '{plugin.Name} v{plugin.Version}'.");
            
            initPlugin.Initialize();
        }
    }
    private void TryExecutePlugin(IPlugin plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
            logger.Log(LogSender.Plugin, LogType.INFO, 
                $"Executing plugin '{plugin.Name} v{plugin.Version}'.");
            
            executablePlugin.Execute();
        }
    }
    private void TryFinalizePlugin(IPlugin plugin)
    {
        if (plugin is IFinalisablePlugin finalPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Finalizing);
            logger.Log(LogSender.Plugin, LogType.INFO, 
                $"Finalizing plugin '{plugin.Name} v{plugin.Version}'.");
            
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
        
        lifecycleManager.SetPluginState(extension.Name, PluginState.Running);
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Executing extension plugin '{extension.Name} v{extension.Version}'.");
        
        extension.Expand(ref data);
        
        TryFinalizePlugin(extension);
    }
    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
    {
        TryInitializePlugin(plugin);
        
        lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
        logger.Log(LogSender.Plugin, LogType.INFO, 
            $"Executing network plugin '{plugin.Name} v{plugin.Version}' | Mode: Receive.");
        
        var result = plugin.ReceiveData();
        
        TryFinalizePlugin(plugin);
        return result;
    }
    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => ExecuteAction(addon =>
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
            logger.Log(LogSender.Plugin, LogType.INFO, 
                $"Executing network plugin '{plugin.Name} v{plugin.Version}' | Mode: Send.");
            
            plugin.SendData(data);
        }, plugin);
}