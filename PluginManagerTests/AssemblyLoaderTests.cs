using ModularPluginAPI.Components;
using Xunit;

namespace PluginManagerTests;

public class AssemblyLoaderTests
{
    private readonly AssemblyLoader _extractor = new(
        "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL");
    
    [Fact]
    public void SetWrongDirectory_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var extractorWrongDirectory = new AssemblyLoader("E:\\SomeFolder");
        });
    }
    
    [Fact]
    public void GetAllAssemblies_ReceivedTwoAssemblies()
    {
        Assert.Equal(4 , _extractor.GetAllAssemblies().Count());
    }
    
    [Fact]
    public void GetAllAssemblies_EmptyDirectory_EmptyList()
    {
        var extractor = new AssemblyLoader(
            "C:\\Users\\kand1s\\Desktop\\C# Projects\\ModularPluginSystem\\PluginManagerTests\\TestsDLL\\Empty");
        var assemblies = extractor.GetAllAssemblies();
        
        Assert.Empty(assemblies);
    }
    
    [Fact]
    public void GetAssembly_ReceivedOneAssembly()
    {
        Assert.NotNull(_extractor.GetAssembly("ConsolePlugin2"));
    }
    
    [Fact]
    public void GetAssembly_InvalidAssemblyName_ThrowsException()
    {
        Assert.Throws<FileNotFoundException>(() => _extractor.GetAssembly("SomeAssembly"));
    }
    
    [Fact]
    public void GetAssemblies_NameWithExtension_ReceivedTwoAssemblies()
    {
        Assert.Equal(2, _extractor.GetAssemblies(["ConsolePlugin2.dll", "ConsolePlugin2"]).Count());
    }
    
    [Fact]
    public void GetAssemblies_InvalidNames_TheListIsEmpty()
    {
        Assert.Empty(_extractor.GetAssemblies([]));
    }
}