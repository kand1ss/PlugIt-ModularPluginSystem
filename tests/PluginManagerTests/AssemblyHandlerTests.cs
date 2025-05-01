using System.Reflection;
using ModularPluginAPI.Components;
using ModularPluginAPI.Exceptions;
using PluginAPI;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyHandlerTests : TestWhichUsingTestAssembly
{
    private const string TestPlugin = "TestPlugin";
    private const string NetworkPlugin = "NetworkPlugin";
    
    private readonly Assembly _testAssembly;
    private readonly AssemblyHandler _handler = new();

    public AssemblyHandlerTests()
    {
        _testAssembly = Assembly.LoadFrom(TestAssemblyPath);
    }

    [Fact]
    public void GetPlugin_CorrectPluginName_ReturnsCorrectPlugin()
    {
        var plugin = _handler.GetPlugin<IPluginData>(_testAssembly, TestPlugin);
        
        Assert.NotNull(plugin);
        Assert.IsAssignableFrom<IPluginData>(plugin);
        Assert.Equal(TestPlugin, plugin.Name);
    }

    [Fact]
    public void GetPlugin_InvalidPluginName_ThrowsException()
    {
        Assert.Throws<PluginNotFoundException>(() => 
            _handler.GetPlugin<IPluginData>(_testAssembly, "InvalidPlugin"));
    }

    [Fact]
    public void GetPlugin_SpecifiedPlugin_ReturnsCorrectPlugin()
    {
        var plugin = _handler.GetPlugin<INetworkPlugin>(_testAssembly, NetworkPlugin);
        
        Assert.NotNull(plugin);
        Assert.IsAssignableFrom<INetworkPlugin>(plugin);
        Assert.Equal("NetworkPlugin", plugin.Name);
    }

    [Fact]
    public void GetPlugin_CorrectNameButNotTheRightOne_ThrowsException()
    {
        Assert.Throws<PluginNotFoundException>(() => 
            _handler.GetPlugin<INetworkPlugin>(_testAssembly, TestPlugin));
    }
}