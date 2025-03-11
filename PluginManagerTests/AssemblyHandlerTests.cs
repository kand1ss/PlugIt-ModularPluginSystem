using System.Reflection;
using ModularPluginAPI.Components;
using ModularPluginAPI.Context;
using PluginAPI;
using Xunit;

namespace PluginManagerTests;

public class AssemblyHandlerTests
{
    private readonly string _assembliesPath =
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\";
    
    private readonly AssemblyHandler _handler = new();
    private readonly PluginLoadContext _loadContext;

    public AssemblyHandlerTests()
    {
        _loadContext = new PluginLoadContext(_assembliesPath);
    }

    [Fact]
    public void GetAllPlugins_ReceivedAllPluginsFromAssembly()
    {
        var assembly = _loadContext.LoadFromAssemblyPath(_assembliesPath + "ConsolePlugin2.dll");
        var plugins = _handler.GetAllPlugins(assembly).ToList();
        
        Assert.NotEmpty(plugins);
        Assert.Single(plugins);
    }

    [Fact]
    public void GetAllPlugins_ReceiverAllPluginsFromAssemblies()
    {
        var assembly1 = _loadContext.LoadFromAssemblyPath(_assembliesPath + "ConsolePlugin2.dll");
        var assembly2 = _loadContext.LoadFromAssemblyPath(_assembliesPath + "ConsolePlugin2.dll");
        var plugins = _handler.GetAllPlugins([assembly1, assembly2]).ToList();
        
        Assert.NotEmpty(plugins);
        Assert.Equal(2, plugins.Count);
    }

    [Fact]
    public void GetPluginsWithGeneric_ReceivedAllPluginsFromAssemblyWithCorrectTypes()
    {
        var assembly = _loadContext.LoadFromAssemblyPath(_assembliesPath + "NetworkPlugin.dll");
        var plugins = _handler.GetPlugins<INetworkPlugin>(assembly).ToList();
        
        Assert.NotEmpty(plugins);
        Assert.Single(plugins);
    }

    [Fact]
    public void GetPluginsWithGeneric_ReceivedAllPluginsFromAssembliesWithCorrectTypes()
    {
        var assembly1 = _loadContext.LoadFromAssemblyPath(_assembliesPath + "NetworkPlugin.dll");
        var assembly2 = _loadContext.LoadFromAssemblyPath(_assembliesPath + "Plugins.dll");
        
        var networkPlugins = _handler.GetPlugins<INetworkPlugin>([assembly1, assembly2]).ToList();
        Assert.NotEmpty(networkPlugins);
        Assert.Equal(3, networkPlugins.Count);
        
        var executablePlugins = _handler.GetPlugins<IExecutablePlugin>([assembly1, assembly2]).ToList();
        Assert.NotEmpty(executablePlugins);
        Assert.Single(executablePlugins);
    }
}