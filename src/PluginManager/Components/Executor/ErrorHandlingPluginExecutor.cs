using ModularPluginAPI.Components.Interfaces;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class ErrorHandlingPluginExecutor(IPluginExecutor pluginExecutor, IPluginTracker pluginTracker, 
    PluginLoggingFacade logger) 
    : IPluginExecutor, IObservableErrorHandledPluginExecutor
{
    private readonly List<IErrorHandledPluginExecutorObserver> _errorObservers = new();
    
    private void NotifyObservers(IPlugin plugin, Exception exception)
    {
        var pluginInfo = pluginTracker.GetPluginStatus(plugin.Name);
        
        foreach(var observer in _errorObservers)
            observer.OnPluginFaulted(pluginInfo, exception);
    }
    public void AddObserver(IErrorHandledPluginExecutorObserver observer)
        => _errorObservers.Add(observer);
    public void RemoveObserver(IErrorHandledPluginExecutorObserver observer)
        => _errorObservers.Remove(observer);

    public void Execute(IPlugin plugin)
    {
        try
        {
            pluginExecutor.Execute(plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
    }

    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> plugin)
    {
        try
        {
            pluginExecutor.ExecuteExtensionPlugin(ref data, plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
    }

    public byte[] ExecuteNetworkPluginReceive(INetworkPlugin plugin)
    {
        try
        {
            return pluginExecutor.ExecuteNetworkPluginReceive(plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
        return [];
    }

    public void ExecuteNetworkPluginSend(byte[] data, INetworkPlugin plugin)
    {
        try
        {
            pluginExecutor.ExecuteNetworkPluginSend(data, plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
    }
}