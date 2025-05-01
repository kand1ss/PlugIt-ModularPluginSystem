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
    private void NotifyObservers(IPlugin plugin, PluginState state)
    {
        var metadata = PluginMetadataGenerator.Generate(plugin);
        var pluginInfo = PluginInfoMapper.Map(metadata);
        pluginInfo.State = state;
        
        foreach(var observer in _observers)
            observer.OnPluginStateChanged(pluginInfo);
    }

    private void TryExecutePlugin(IPlugin plugin)
    {
        if (plugin is IExecutablePlugin executablePlugin)
        {
            logger.PluginExecuted(plugin.Name, plugin.Version);
            executablePlugin.Execute();
        }
    }

    private void OnPluginCompleted(IPlugin plugin)
    {
        NotifyObservers(plugin, PluginState.Completed);
        logger.PluginExecutionCompleted(plugin.Name, plugin.Version);
    }

    private void ExecutePlugin(IPlugin plugin, Action<IPlugin> action)
    {
        NotifyObservers(plugin, PluginState.Running);
        action(plugin);
        
        OnPluginCompleted(plugin);
    }

    private TResult ExecutePlugin<TResult>(IPlugin plugin, Func<IPlugin, TResult> action)
    {
        NotifyObservers(plugin, PluginState.Running);
        var result = action(plugin);
        
        OnPluginCompleted(plugin);
        return result;
    }


    public void Execute(IPlugin plugin)
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
            logger.NetworkPluginExecuting(p.Name, p.Version, false);
            return await ((INetworkPlugin)p).ReceiveDataAsync();
        });

    public async Task ExecuteNetworkPluginSendAsync(byte[] data, INetworkPlugin plugin)
        => await ExecutePlugin(plugin, async p =>
        {
            logger.NetworkPluginExecuting(p.Name, p.Version, true);
            await ((INetworkPlugin)p).SendDataAsync(data);
        });

    public async Task<byte[]> ExecuteFilePluginReadAsync(IFilePlugin plugin)
        => await ExecutePlugin<Task<byte[]>>(plugin, async p =>
        {
            logger.FilePluginExecuting(p.Name, p.Version, false);;
            return await ((IFilePlugin)p).ReadFileAsync();
        });
    
    public async Task ExecuteFilePluginWriteAsync(byte[] data, IFilePlugin plugin)
        => await ExecutePlugin(plugin, async p =>
        {
            logger.FilePluginExecuting(p.Name, p.Version, true);;
            await ((IFilePlugin)p).WriteFileAsync(data);
        });
}