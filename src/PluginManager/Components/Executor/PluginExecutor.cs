using ModularPluginAPI.Components.Interfaces;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class PluginExecutor(PluginLoggingFacade logger) : IPluginExecutor, IObservablePluginExecutor
{
    private readonly List<IPluginExecutorObserver> _observers = new();
    
    public void AddObserver(IPluginExecutorObserver observer)
        => _observers.Add(observer);
    public void RemoveObserver(IPluginExecutorObserver observer)
        => _observers.Remove(observer);
    private void NotifyObservers(IPluginData plugin, PluginState state, PluginMode mode = PluginMode.Fixed)
    {
        var metadata = PluginMetadataGenerator.Generate(plugin);
        var pluginInfo = PluginStatusMapper.Map(metadata, state, mode);
        
        foreach(var observer in _observers)
            observer.OnPluginStatusChanged(pluginInfo);
    }

    private void TryExecutePlugin(IPluginData plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            logger.PluginExecuted(plugin.Name, plugin.Version);
            executablePlugin.Execute();
        }
    }

    private void OnPluginCompleted(IPluginData plugin)
    {
        NotifyObservers(plugin, PluginState.Completed, PluginMode.Idle);
        logger.PluginExecutionCompleted(plugin.Name, plugin.Version);
    }

    private void ExecutePlugin(IPluginData plugin, Action<IPluginData> action, PluginMode executionMode = PluginMode.Fixed)
    {
        NotifyObservers(plugin, PluginState.Running, executionMode);
        action(plugin);
        
        OnPluginCompleted(plugin);
    }

    private TResult ExecutePlugin<TResult>(IPluginData plugin, Func<IPluginData, TResult> action, PluginMode executionMode = PluginMode.Fixed)
    {
        NotifyObservers(plugin, PluginState.Running, executionMode);
        var result = action(plugin);
        
        OnPluginCompleted(plugin);
        return result;
    }


    public void Execute(IPluginData plugin)
        => ExecutePlugin(plugin, TryExecutePlugin);

    public void ExecuteExtensionPlugin<T>(ref T data, IExtensionPlugin<T> plugin)
    {
        NotifyObservers(plugin, PluginState.Running);
        logger.ExtensionPluginExecuting(plugin.Name, plugin.Version);
        plugin.Expand(ref data);
        
        OnPluginCompleted(plugin);
    }

    public async Task<byte[]> ExecuteNetworkPluginReceiveAsync(INetworkPlugin plugin)
        => await ExecutePlugin<Task<byte[]>>(plugin, async p =>
        {
            logger.NetworkPluginExecuting(p.Name, p.Version, PluginMode.Receive);
            return await ((INetworkPlugin)p).ReceiveDataAsync();
        }, PluginMode.Receive);

    public async Task ExecuteNetworkPluginSendAsync(byte[] data, INetworkPlugin plugin)
        => await ExecutePlugin(plugin, async p =>
        {
            logger.NetworkPluginExecuting(p.Name, p.Version, PluginMode.Send);
            await ((INetworkPlugin)p).SendDataAsync(data);
        }, PluginMode.Send);

    public async Task<byte[]> ExecuteFilePluginReadAsync(IFilePlugin plugin)
        => await ExecutePlugin<Task<byte[]>>(plugin, async p =>
        {
            logger.FilePluginExecuting(p.Name, p.Version, PluginMode.Receive);;
            return await ((IFilePlugin)p).ReadFileAsync();
        }, PluginMode.Receive);
    
    public async Task ExecuteFilePluginWriteAsync(byte[] data, IFilePlugin plugin)
        => await ExecutePlugin(plugin, async p =>
        {
            logger.FilePluginExecuting(p.Name, p.Version, PluginMode.Send);;
            await ((IFilePlugin)p).WriteFileAsync(data);
        }, PluginMode.Send);
}