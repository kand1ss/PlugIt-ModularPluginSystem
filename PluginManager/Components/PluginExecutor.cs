using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(IPluginLifecycleManager lifecycleManager, PluginLoggingFacade logger) : IPluginExecutor
{
    private void TryInitializePlugin(IPlugin plugin)
    {
        if (plugin is IInitialisablePlugin initPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Initializing);
            logger.PluginInitialized(plugin.Name, plugin.Version);
            
            initPlugin.Initialize();
        }
    }
    private void TryExecutePlugin(IPlugin plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
            logger.PluginExecuted(plugin.Name, plugin.Version);
            
            executablePlugin.Execute();
        }
    }
    private void TryFinalizePlugin(IPlugin plugin)
    {
        if (plugin is IFinalisablePlugin finalPlugin)
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Finalizing);
            logger.PluginFinalized(plugin.Name, plugin.Version);
            
            finalPlugin.FinalizePlugin();
        }
    }
    private ExecutionResult ExecuteAction(Action<IPlugin> action, IPlugin plugin)
    {
        TryInitializePlugin(plugin);
        action(plugin);
        TryFinalizePlugin(plugin);
        return ExecutionResult.Success();
    }
    
    
    public ExecutionResult Execute(IPlugin plugin)
        => ExecuteAction(TryExecutePlugin, plugin);

    public ExecutionResult ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> extension)
    {
        TryInitializePlugin(extension);
        
        lifecycleManager.SetPluginState(extension.Name, PluginState.Running);
        logger.ExtensionPluginExecuting(extension.Name, extension.Version);
        
        extension.Expand(ref data);
        
        TryFinalizePlugin(extension);
        return ExecutionResult.Success();
    }
    public ExecutionResult ExecuteNetworkPluginReceive(INetworkPlugin plugin, out byte[] response)
    {
        TryInitializePlugin(plugin);
        
        lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
        logger.NetworkPluginExecuting(plugin.Name, plugin.Version, false);
        
        response = plugin.ReceiveData();
        
        TryFinalizePlugin(plugin);
        return ExecutionResult.Success();
    }
    public ExecutionResult ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => ExecuteAction(addon =>
        {
            lifecycleManager.SetPluginState(plugin.Name, PluginState.Running);
            logger.NetworkPluginExecuting(plugin.Name, plugin.Version, true);
            
            plugin.SendData(data);
        }, plugin);
}