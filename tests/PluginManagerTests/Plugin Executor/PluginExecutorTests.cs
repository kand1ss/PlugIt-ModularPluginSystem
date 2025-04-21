using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using Moq;
using PluginManagerTests.Test_Plugins;
using TestAssembly;
using Xunit;

namespace PluginManagerTests;

public class PluginExecutorTests
{
    private readonly StateObserver _executorObserver = new();
    private readonly PluginExecutor _pluginExecutor;

    public PluginExecutorTests()
    {
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        _pluginExecutor = new(logger);
        _pluginExecutor.AddObserver(_executorObserver);
    }

    [Fact]
    public void Execute_CommonPlugin_PluginSuccessfullyExecuted()
    {
        var plugin = new TestFullPlugin();
        _pluginExecutor.Execute(plugin);
        
        Assert.True(plugin.IsInitialized);
        Assert.True(plugin.IsExecuted);
        Assert.True(plugin.IsFinalized);
        
        Assert.Equal(new[]
        {
            PluginState.Initializing,
            PluginState.Running,
            PluginState.Finalizing,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }

    [Fact]
    public void ExecuteExtensionPlugin_CommonPlugin_PluginSuccessfullyExecuted()
    {
        var plugin = new TestExtensionPlugin();
        var data = "data";
        _pluginExecutor.ExecuteExtensionPlugin(ref data, plugin);
        
        Assert.True(plugin.IsInitialized);
        Assert.True(plugin.IsExecuted);
        Assert.True(plugin.IsFinalized);
        
        Assert.Equal(new[]
        {
            PluginState.Initializing,
            PluginState.Running,
            PluginState.Finalizing,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }

    [Fact]
    public async Task ExecuteNetworkPlugin_SendMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestNetworkPlugin();
        var randomData = new byte[101];
        await _pluginExecutor.ExecuteNetworkPluginSendAsync(randomData, plugin);
        
        Assert.True(plugin.SendCalled);
        Assert.False(plugin.ReceiveCalled);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }
    
    [Fact]
    public async Task ExecuteNetworkPlugin_ReceiveMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestNetworkPlugin();
        await _pluginExecutor.ExecuteNetworkPluginReceiveAsync(plugin);
        
        Assert.False(plugin.SendCalled);
        Assert.True(plugin.ReceiveCalled);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }

    [Fact]
    public async Task ExecuteFilePlugin_ReadMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestFilePlugin();
        await _pluginExecutor.ExecuteFilePluginReadAsync(plugin);
        
        Assert.True(plugin.IsRead);
        Assert.False(plugin.IsWrite);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }
    
    [Fact]
    public async Task ExecuteFilePlugin_WriteMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestFilePlugin();
        await _pluginExecutor.ExecuteFilePluginWriteAsync([], plugin);
        
        Assert.False(plugin.IsRead);
        Assert.True(plugin.IsWrite);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }
}