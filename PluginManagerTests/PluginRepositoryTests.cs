using System.Reflection;
using ModularPluginAPI.Components;
using PluginAPI;
using PluginManagerTests.PluginsForTests;
using Xunit;

namespace PluginManagerTests;

public class PluginRepositoryTests
{
    private readonly PluginRepository _repository = new();

    [Fact]
    public void Add_PluginAddedToRepository()
    {
        _repository.Add(new ConsolePlugin());
        Assert.NotEmpty(_repository.GetAll());
    }

    [Fact]
    public void AddRange_PluginsAddedToRepository()
    {
        var networkPlugin = new NetworkPlugin();
        var consolePlugin = new ConsolePlugin();
        
        _repository.AddRange([networkPlugin, consolePlugin]);
        Assert.NotEmpty(_repository.GetAll());
        Assert.Equal(2, _repository.GetAll().Count());
    }

    [Fact]
    public void AddRange_PluginWithSameNameWillNotAdd()
    {
        _repository.Add(new ConsolePlugin());
        _repository.Add(new ConsolePlugin());
        Assert.Single(_repository.GetAll());
    }

    [Fact]
    public void Remove_PluginRemovedFromRepository()
    {
        var plugin = new ConsolePlugin();
        _repository.Add(plugin);
        _repository.Remove(plugin.Name);
        
        Assert.Empty(_repository.GetAll());
    }

    [Fact]
    public void GetByName_ReceivedCorrectPlugin()
    {
        _repository.Add(new ConsolePlugin());
        Assert.NotNull(_repository.GetByName("ConsolePlugin"));
    }

    [Fact]
    public void GetAllWithGeneric_ReceivedAllPluginsWithCorrectType()
    {
        _repository.Add(new ConsolePlugin());
        _repository.Add(new NetworkPlugin());
        _repository.Add(new NetworkPlugin2());
        
        Assert.Equal(2, _repository.GetAll<INetworkPlugin>().Count());
    }

    [Fact]
    public void GetByNameWithGeneric_ReceivedCorrectPlugin()
    {
        _repository.Add(new ConsolePlugin());
        _repository.Add(new NetworkPlugin());
        
        Assert.NotNull(_repository.GetByName<INetworkPlugin>("NetworkPlugin"));
    }
}