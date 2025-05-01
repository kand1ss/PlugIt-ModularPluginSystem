using ModularPluginAPI.Components.Interfaces;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class ErrorHandlingPluginExecutor(IPluginExecutor pluginExecutor, IPluginTracker pluginTracker, 
    PluginLoggingFacade logger) 
    : IPluginExecutor, IObservableErrorHandledPluginExecutor
{
    private readonly List<IErrorHandledPluginExecutorObserver> _errorObservers = new();
    
    private void NotifyObservers(IPluginData plugin, Exception exception)
    {
        var metadata = PluginMetadataGenerator.Generate(plugin);
        var pluginInfo = PluginStatusMapper.Map(metadata);
        
        var pluginStateFromTracker = pluginTracker.GetPluginStatus(plugin.Name);
        if(pluginStateFromTracker is not null)
            pluginInfo.CurrentState = pluginStateFromTracker.CurrentState;
        
        foreach(var observer in _errorObservers)
            observer.OnPluginFaulted(pluginInfo, exception);
    }
    public void AddObserver(IErrorHandledPluginExecutorObserver observer)
        => _errorObservers.Add(observer);
    public void RemoveObserver(IErrorHandledPluginExecutorObserver observer)
        => _errorObservers.Remove(observer);

    public void Execute(IPluginData plugin)
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

    public async Task<byte[]> ExecuteNetworkPluginReceiveAsync(INetworkPlugin plugin)
    {
        try
        {
            return await pluginExecutor.ExecuteNetworkPluginReceiveAsync(plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
        return [];
    }

    public async Task ExecuteNetworkPluginSendAsync(byte[] data, INetworkPlugin plugin)
    {
        try
        {
            await pluginExecutor.ExecuteNetworkPluginSendAsync(data, plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
    }

    public async Task<byte[]> ExecuteFilePluginReadAsync(IFilePlugin plugin)
    {
        try
        {
            return await pluginExecutor.ExecuteFilePluginReadAsync(plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }

        return [];
    }

    public async Task ExecuteFilePluginWriteAsync(byte[] data, IFilePlugin plugin)
    {
        try
        {
            await pluginExecutor.ExecuteFilePluginWriteAsync(data, plugin);
        }
        catch (Exception e)
        {
            logger.PluginFaulted(plugin.Name, e.Message);
            NotifyObservers(plugin, e);
        }
    }
}