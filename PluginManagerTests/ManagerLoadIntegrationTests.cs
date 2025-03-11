using ModularPluginAPI.Components;
using PluginAPI;
using Xunit;

namespace PluginManagerTests;

public class ManagerLoadIntegrationTests
{
    private readonly AssemblyLoader _loader = new(
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL");
    private readonly AssemblyHandler _handler = new();
    private readonly AssemblyMetadataRepository _metadataRepository = new();
    private readonly AssemblyMetadataGenerator _metadataGenerator;

    public ManagerLoadIntegrationTests()
    {
        _metadataGenerator = new(_handler);
    }
    
    [Fact]
    public void ExtractAssemblies_GetMetadataThenGetPlugins_PlaceMetadataToRepository()
    {
        var assemblies = _loader.GetAllAssemblies().ToList();
        var metadata = _metadataGenerator.Generate(assemblies);
        var allPlugins = _handler.GetAllPlugins(assemblies).ToList();
        
        Assert.Equal(7, allPlugins.Count);
        
        _metadataRepository.AddRange(metadata);
        Assert.NotEmpty(_metadataRepository.GetAllMetadata());
    }
    
    [Fact]
    public void ExtractAssembly_GetMetadataThenGetSpecifiedPlugins_PlaceMetadataToRepository()
    {
        var assemblies = _loader.GetAllAssemblies().ToList();
        var metadata = _metadataGenerator.Generate(assemblies);
        var networkPlugins = _handler.GetPlugins<INetworkPlugin>(assemblies).ToList();
        
        Assert.Equal(3, networkPlugins.Count);
        
        _metadataRepository.AddRange(metadata);
        Assert.NotEmpty(_metadataRepository.GetAllMetadata());
    }
}