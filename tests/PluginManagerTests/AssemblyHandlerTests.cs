using System.Reflection;
using ModularPluginAPI.Components;
using ModularPluginAPI.Exceptions;
using PluginAPI;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyHandlerTests : TestWhichUsingTestAssembly
{
    private readonly Assembly _testAssembly;
    private readonly AssemblyHandler _handler = new();

    public AssemblyHandlerTests()
    {
        _testAssembly = Assembly.LoadFrom(TestAssemblyPath);
    }

    [Fact]
    public void GetPlugin_CorrectPluginName_ReturnsCorrectPlugin()
    {
        var plugin = _handler.GetPlugin<IPlugin>(_testAssembly, "TestPlugin");
        Assert.NotNull(plugin);
        Assert.IsAssignableFrom<IPlugin>(plugin);
        Assert.Equal("TestPlugin", plugin.Name);

    }

    [Fact]
    public void GetPlugin_InvalidPluginName_ThrowsException()
    {
        Assert.Throws<PluginNotFoundException>(() => 
            _handler.GetPlugin<IPlugin>(_testAssembly, "InvalidPlugin"));
    }

    [Fact]
    public void GetPlugin_SpecifiedPlugin_ReturnsCorrectPlugin()
    {
        var plugin = _handler.GetPlugin<INetworkPlugin>(_testAssembly, "NetworkPlugin");
        Assert.NotNull(plugin);
    }

    [Fact]
    public void GetPlugin_CorrectNameButNotTheRightOne_ThrowsException()
    {
        Assert.Throws<PluginNotFoundException>(() => 
            _handler.GetPlugin<INetworkPlugin>(_testAssembly, "TestPlugin"));
    }

    [Fact]
    public void GetAllPlugins_ReturnsAllPlugins()
    {
        var plugins = _handler.GetAllPlugins(_testAssembly).ToList();
        Assert.Contains(plugins, p => p.GetType().Name == "TestPlugin");
        Assert.Contains(plugins, p => p is IPlugin);

    }
}