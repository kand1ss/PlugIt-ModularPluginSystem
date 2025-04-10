using System.Data;
using ModularPluginAPI.Components;
using ModularPluginAPI.Models;
using Xunit;

namespace PluginManagerTests;

public class AssemblyMetadataRepositoryTests
{
    private readonly string _testMetadataPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly AssemblyMetadata _testMetadata;
    private readonly AssemblyMetadataRepository _repository = new();

    
    public AssemblyMetadataRepositoryTests()
    {
        _testMetadata = new AssemblyMetadata
        {
            Name = "SomeAssembly", Path = _testMetadataPath, Plugins =
            {
                new PluginMetadata { Name = "Plugin" },
                new PluginMetadata { Name = "Plugin2" }
            }
        };
    }
    
    
    [Fact]
    public void Add_CommonMetadata_ShouldAddMetadata()
    {
        _repository.Add(_testMetadata);
        Assert.Same(_testMetadata, _repository.GetMetadataByAssemblyPath(_testMetadataPath));
    }

    [Fact]
    public void Add_MetadataHasPluginThatAlreadyExists_ThrowsException()
    {
        _repository.Add(_testMetadata);
        _testMetadata.Name = "SomeOtherAssembly";
        Assert.Throws<DuplicateNameException>(() => _repository.Add(_testMetadata));
    }

    [Fact]
    public void Add_DuplicateAssembly_NotAdded()
    {
        var metadata = new AssemblyMetadata { Name = _testMetadata.Name, Path = _testMetadataPath };
        _repository.Add(_testMetadata);
        _repository.Add(metadata);
        
        Assert.Single(_repository.GetAllMetadata());
    }

    [Fact]
    public void Remove_CorrectAssemblyPath_ShouldRemoveMetadata()
    {
        _repository.Add(_testMetadata);
        _repository.Remove(_testMetadata.Path);
        
        Assert.Empty(_repository.GetAllMetadata());
    }

    [Fact]
    public void Remove_FakeAssemblyPath_NotRemoved()
    {
        _repository.Add(_testMetadata);
        _repository.Remove(Path.Combine("D", "FakePath"));
        
        Assert.NotEmpty(_repository.GetAllMetadata());
    }

    [Fact]
    public void Clear_RepositoryCleared()
    {
        var metadata = new AssemblyMetadata { Name = "SomeOtherAssembly", Path = Path.Combine("D", "FakePath") };
        _repository.Add(_testMetadata);
        _repository.Add(metadata);
        
        _repository.Clear();
        Assert.Empty(_repository.GetAllMetadata());
    }
}