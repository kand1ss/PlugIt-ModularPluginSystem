using ModularPluginAPI.Components;
using Xunit;

namespace PluginManagerTests;

public class PluginExtractorTests
{
    private readonly AssemblyExtractor _extractor = new(
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL");

    [Fact]
    public void GetAllFromDirectory_ReceivedTwoAssemblies()
    {
        Assert.Equal(4 , _extractor.GetAllFromDirectory().Count());
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
    public void GetFromDirectory_ReceivedTwoAssemblies()
    {
        Assert.Equal(2, _extractor.GetAssemblies([
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\ConsolePlugin", 
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\ConsolePlugin2"]).Count());
    }
    
    [Fact]
    public void GetFromDirectory_InvalidNames_TheListIsEmpty()
    {
        Assert.Empty(_extractor.GetAssemblies([]));
    }
}