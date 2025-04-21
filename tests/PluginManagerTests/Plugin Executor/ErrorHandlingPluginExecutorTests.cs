using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Logger;
using Moq;
using TestAssembly;
using Xunit;

namespace PluginManagerTests;

public class ErrorHandlingPluginExecutorTests
{
    private readonly PluginExecutor _pluginExecutor;
    private readonly ErrorHandlingPluginExecutor _errorHandledExecutor;
    private readonly ExceptionObserver _exceptionObserver = new();

    public ErrorHandlingPluginExecutorTests()
    {
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        var tracker = new Mock<IPluginTracker>();

        _pluginExecutor = new(logger);
        _errorHandledExecutor = new(_pluginExecutor, tracker.Object, logger);
        _errorHandledExecutor.AddObserver(_exceptionObserver);
    }


    [Fact]
    public void Execute_FaultedPlugin_ExceptionHandled()
    {
        var faultedPlugin = new FaultedPlugin();
        _errorHandledExecutor.Execute(faultedPlugin);
        
        Assert.NotEmpty(_exceptionObserver.AddedErrors);
        Assert.Contains(faultedPlugin.Name, _exceptionObserver.AddedErrors);
    }

    [Fact]
    public void ExecuteExtensionPlugin_FaultedPlugin_ExceptionHandled()
    {
        var faultedPlugin = new FaultedExtensionPlugin();
        var data = "Data";
        _errorHandledExecutor.ExecuteExtensionPlugin(ref data, faultedPlugin);
        
        Assert.NotEmpty(_exceptionObserver.AddedErrors);
        Assert.Contains(faultedPlugin.Name, _exceptionObserver.AddedErrors);
    }

    [Fact]
    public async Task ExecuteNetworkPluginSend_FaultedPlugin_ExceptionHandled()
    {
        var faultedPlugin = new FaultedNetworkPlugin();
        await _errorHandledExecutor.ExecuteNetworkPluginSendAsync([101, 52], faultedPlugin);
        
        Assert.NotEmpty(_exceptionObserver.AddedErrors);
        Assert.Contains(faultedPlugin.Name, _exceptionObserver.AddedErrors);
    }
    
    [Fact]
    public async Task ExecuteNetworkPluginReceive_FaultedPlugin_ExceptionHandled()
    {
        var faultedPlugin = new FaultedNetworkPlugin();
        await _errorHandledExecutor.ExecuteNetworkPluginReceiveAsync(faultedPlugin);
        
        Assert.NotEmpty(_exceptionObserver.AddedErrors);
        Assert.Contains(faultedPlugin.Name, _exceptionObserver.AddedErrors);
    }

    [Fact]
    public async Task ExecuteFilePluginRead_FaultedPlugin_ExceptionHandled()
    {
        var plugin = new FaultedFilePlugin();
        await _errorHandledExecutor.ExecuteFilePluginReadAsync(plugin);
        
        Assert.NotEmpty(_exceptionObserver.AddedErrors);
        Assert.Contains(plugin.Name, _exceptionObserver.AddedErrors);
    }
}