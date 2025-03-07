using ModularPluginAPI.Components;
using PluginAPI;
using Xunit;

namespace PluginManagerTests;

public class PluginLoadIntegrationTests
{
    private readonly AssemblyExtractor _extractor = new(
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL");
    
    private readonly AssemblyHandler _handler = new();
    private readonly PluginRepository _repository = new();

    [Fact]
    public void ExtractAssembly_ExtractPluginsFromAssembly_PlaceToRepository()
    {
        var assemblies = _extractor.GetAllFromDirectory();
        var allPlugins = _handler.GetAllPlugins(assemblies).ToList();
        
        Assert.NotEmpty(allPlugins);
        
        _repository.AddRange(allPlugins);
        
        Assert.NotEmpty(_repository.GetAll());
    }

    [Fact]
    public void ExtractAssembly_ExtractSpecifiedPluginsFromAssembly_PlaceToRepository()
    {
        var assemblies = _extractor.GetAllFromDirectory();
        var networkPlugins = _handler.GetPlugins<INetworkPlugin>(assemblies).ToList();
        
        Assert.NotEmpty(networkPlugins);
        
        _repository.AddRange(networkPlugins);
        
        Assert.NotEmpty(_repository.GetAll());
    }
}