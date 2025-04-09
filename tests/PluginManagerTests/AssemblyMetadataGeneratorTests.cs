using System.Reflection;
using ModularPluginAPI.Components;
using Moq;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyMetadataGeneratorTests : TestWhichUsingTestAssembly
{
    private readonly Assembly _testAssembly;
    private readonly AssemblyMetadataGenerator _metadataGenerator;

    public AssemblyMetadataGeneratorTests()
    {
        _testAssembly = Assembly.LoadFrom(TestAssemblyPath);
        var handlerMock = new Mock<IAssemblyHandler>();
        _metadataGenerator = new(handlerMock.Object);
    }

    [Fact]
    public void Generate_MetadataHasCorrectData()
    {
        var metadata = _metadataGenerator.Generate(_testAssembly);
        
        Assert.Equal(metadata.Path, _testAssembly.Location);
        Assert.Equal(metadata.Version, _testAssembly.GetName().Version);
        Assert.Equal(metadata.Name, _testAssembly.GetName().Name);
        Assert.Equal(metadata.Author, _testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company);
    }
}