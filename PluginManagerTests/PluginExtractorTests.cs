using ModularPluginAPI.Components;
using Xunit;

namespace PluginManagerTests;

public class PluginExtractorTests
{
    private readonly AssemblyExtractor _extractor = new(
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL");

    [Fact]
    public void SetWrongDirectory_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var extractorWrongDirectory = new AssemblyExtractor("E:\\SomeFolder");
        });
    }

    [Fact]
    public void GetAllFromDirectory_ReceivedTwoAssemblies()
    {
        Assert.Equal(4 , _extractor.GetAllFromDirectory().Count());
    }

    [Fact]
    public void GetAllFromDirectory_EmptyDirectory_EmptyList()
    {
        var extractor = new AssemblyExtractor(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\Empty");
        var assemblies = extractor.GetAllFromDirectory();
        
        Assert.Empty(assemblies);
    }

    [Fact]
    public void GetFromDirectory_ReceivedOneAssembly()
    {
        Assert.NotNull(_extractor.GetAssembly("ConsolePlugin"));
    }
    
    [Fact]
    public void GetFromDirectory_InvalidAssemblyName_ThrowsException()
    {
        Assert.Throws<FileNotFoundException>(() => _extractor.GetAssembly("SomeAssembly"));
    }
    
    [Fact]
    public void GetFromDirectory_NameWithExtension_ReceivedTwoAssemblies()
    {
        Assert.Equal(2, _extractor.GetAssemblies(["ConsolePlugin.dll", "ConsolePlugin2"]).Count());
    }

    [Fact]
    public void GetFromDirectory_InvalidNames_TheListIsEmpty()
    {
        Assert.Empty(_extractor.GetAssemblies([]));
    }
}