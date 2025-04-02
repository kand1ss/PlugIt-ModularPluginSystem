using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(PluginLoggingFacade logger) : IPluginExecutor
{
    private readonly List<IPluginExecutorObserver> _observers = new();
    
    public void AddObserver(IPluginExecutorObserver observer)
        => _observers.Add(observer);

    public void RemoveObserver(IPluginExecutorObserver observer)
        => _observers.Remove(observer);

    private void NotifyObservers(IPlugin plugin, PluginState state)
    {
        var metadata = PluginMetadataGenerator.Generate(plugin);
        var pluginInfo = PluginInfoMapper.Map(metadata);
        pluginInfo.State = state;
        
        foreach(var observer in _observers)
            observer.OnPluginStateChanged(pluginInfo);
    }

    
    
    private void TryInitializePlugin(IPlugin plugin)
    {
        if (plugin is IInitialisablePlugin initPlugin)
        {
            NotifyObservers(plugin, PluginState.Initializing);
            logger.PluginInitialized(plugin.Name, plugin.Version);
            initPlugin.Initialize();
        }
    }

    private void TryExecutePlugin(IPlugin plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            logger.PluginExecuted(plugin.Name, plugin.Version);
            executablePlugin.Execute();
        }
    }

    private void TryFinalizePlugin(IPlugin plugin)
    {
        if (plugin is IFinalisablePlugin finalPlugin)
        {
            NotifyObservers(plugin, PluginState.Finalizing);
            logger.PluginFinalized(plugin.Name, plugin.Version);
            finalPlugin.FinalizePlugin();
        }
    }

    private void OnPluginCompleted(IPlugin plugin)
    {
        NotifyObservers(plugin, PluginState.Completed);
        logger.PluginExecutionCompleted(plugin.Name, plugin.Version);
    }

    private void ExecutePlugin(IPlugin plugin, Action<IPlugin> action)
    {
        TryInitializePlugin(plugin);
        
        NotifyObservers(plugin, PluginState.Running);
        action(plugin);
        
        TryFinalizePlugin(plugin);
        OnPluginCompleted(plugin);
    }

    private TResult ExecutePlugin<TResult>(IPlugin plugin, Func<IPlugin, TResult> action)
    {
        TryInitializePlugin(plugin);
        
        NotifyObservers(plugin, PluginState.Running);
        var result = action(plugin);
        
        TryFinalizePlugin(plugin);
        OnPluginCompleted(plugin);
        return result;
    }


    public void Execute(IPlugin plugin)
        => ExecutePlugin(plugin, TryExecutePlugin);

    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> plugin)
    {
        TryInitializePlugin(plugin);
        
        NotifyObservers(plugin, PluginState.Running);
        logger.ExtensionPluginExecuting(plugin.Name, plugin.Version);
        plugin.Expand(ref data);
        
        TryFinalizePlugin(plugin);
        OnPluginCompleted(plugin);
    }

    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
        => ExecutePlugin<byte[]>(plugin, p =>
        {
            logger.NetworkPluginExecuting(p.Name, p.Version, false);
            return ((INetworkPlugin)p).ReceiveData();
        });

    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => ExecutePlugin(plugin, p =>
        {
            logger.NetworkPluginExecuting(p.Name, p.Version, true);
            ((INetworkPlugin)p).SendData(data);
        });
}