using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using Moq;
using PluginManagerTests.Test_Plugins;
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
    public void ExecuteNetworkPlugin_SendMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestNetworkPlugin();
        var randomData = new byte[101];
        _pluginExecutor.ExecuteNetworkPluginSend(randomData, plugin);
        
        Assert.True(plugin.SendCalled);
        Assert.False(plugin.ReceiveCalled);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }
    
    [Fact]
    public void ExecuteNetworkPlugin_ReceiveMode_PluginSuccessfullyExecuted()
    {
        var plugin = new TestNetworkPlugin();
        _pluginExecutor.ExecuteNetworkPluginReceive(plugin);
        
        Assert.False(plugin.SendCalled);
        Assert.True(plugin.ReceiveCalled);
        
        Assert.Equal(new[]
        {
            PluginState.Running,
            PluginState.Completed
        }, _executorObserver.ReceivedStates);
    }
}