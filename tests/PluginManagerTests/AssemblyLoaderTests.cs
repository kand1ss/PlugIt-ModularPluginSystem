using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Exceptions;
using Moq;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyLoaderTests : TestWhichUsingTestAssembly
{
    private readonly AssemblyLoader _loader;

    public AssemblyLoaderTests()
    {
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        _loader = new AssemblyLoader(logger);
    }

    [Fact]
    public void LoadAssembly_ValidPath_LoadsAndLogsAssembly()
    {
        var assembly = _loader.LoadAssembly(TestAssemblyPath);
        Assert.NotNull(assembly);
    }

    [Fact]
    public void LoadAssembly_NonExistentPath_ThrowsException()
    {
        Assert.Throws<AssemblyNotFoundException>(() => _loader.LoadAssembly(@"D:\UnknownFile.dll"));
    }

    [Fact]
    public void LoadAssembly_CalledTwice_UsesCachedContext()
    {
        var firstLoad = _loader.LoadAssembly(TestAssemblyPath);
        var secondLoad = _loader.LoadAssembly(TestAssemblyPath);

        Assert.Same(firstLoad, secondLoad);
    }

    [Fact]
    public void UnloadAssembly_LoadAssemblyTwice_AssembliesNotSame()
    {
        var firstLoad = _loader.LoadAssembly(TestAssemblyPath);
        _loader.UnloadAssembly(TestAssemblyPath);
        var secondLoad = _loader.LoadAssembly(TestAssemblyPath);
        
        Assert.NotSame(firstLoad, secondLoad);
    }
}
