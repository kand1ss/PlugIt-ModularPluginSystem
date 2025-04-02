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
            NotifyObservers(plugin, PluginState.Running);
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
        
        NotifyObservers(extension, PluginState.Running);
        logger.ExtensionPluginExecuting(extension.Name, extension.Version);
        
        extension.Expand(ref data);
        
        TryFinalizePlugin(extension);
    }

    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
    {
        TryInitializePlugin(plugin);
        
        NotifyObservers(plugin, PluginState.Running);
        logger.NetworkPluginExecuting(plugin.Name, plugin.Version, false);
        
        var result = plugin.ReceiveData();
        
        TryFinalizePlugin(plugin);
        return result;
    }

    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
        => ExecuteAction(addon =>
        {
            NotifyObservers(plugin, PluginState.Running);
            logger.NetworkPluginExecuting(plugin.Name, plugin.Version, true);
            
            plugin.SendData(data);
        }, plugin);
}