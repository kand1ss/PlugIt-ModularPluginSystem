using System.Reflection;
using ModularPluginAPI.Components;
using ModularPluginAPI.Context;
using PluginAPI;
using Xunit;

namespace PluginManagerTests;

public class AssemblyHandlerTests
{
    private readonly AssemblyHandler _handler = new();

    [Fact]
    public void GetAllPlugins_ReceivedAllPluginsFromAssembly()
    {
        var assembly = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\ConsolePlugin.dll");
        
        var plugins = _handler.GetAllPlugins(assembly).ToList();
        Assert.NotEmpty(plugins);
        Assert.Single(plugins);
    }

    [Fact]
    public void GetAllPlugins_ReceiverAllPluginsFromAssemblies()
    {
        var assembly1 = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\ConsolePlugin.dll");
        var assembly2 = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\ConsolePlugin2.dll");

        var plugins = _handler.GetAllPlugins([assembly1, assembly2]).ToList();
        Assert.NotEmpty(plugins);
        Assert.Equal(2, plugins.Count);
    }

    [Fact]
    public void GetPluginsWithGeneric_ReceivedAllPluginsFromAssemblyWithCorrectTypes()
    {
        var assembly = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\NetworkPlugin.dll");

        var plugins = _handler.GetPlugins<INetworkPlugin>(assembly).ToList();
        Assert.NotEmpty(plugins);
        Assert.Single(plugins);
    }

    [Fact]
    public void GetPluginsWithGeneric_ReceivedAllPluginsFromAssembliesWithCorrectTypes()
    {
        var assembly1 = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\NetworkPlugin.dll");
        var assembly2 = Assembly.LoadFile(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\Plugins.dll");
        
        var networkPlugins = _handler.GetPlugins<INetworkPlugin>([assembly1, assembly2]).ToList();
        Assert.NotEmpty(networkPlugins);
        Assert.Equal(2, networkPlugins.Count);
        
        var executablePlugins = _handler.GetPlugins<IExecutablePlugin>([assembly1, assembly2]).ToList();
        Assert.NotEmpty(executablePlugins);
        Assert.Equal(3, executablePlugins.Count);
    }
}