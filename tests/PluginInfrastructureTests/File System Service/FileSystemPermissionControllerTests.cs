using PluginAPI.Services;
using PluginInfrastructure;

namespace PluginManagerTests;

public class FileSystemPermissionControllerTests
{
    private readonly FileSystemPermissionController _controller = new();

    [Fact]
    public void AddAllowedDirectory_CorrectDirectory_PathAdded()
    {
        var path = Directory.GetCurrentDirectory();
        _controller.AddAllowedDirectory(path);

        Assert.Single(_controller.GetAllowedDirectories());
        Assert.Contains(Normalizer.NormalizeDirectoryPath(path), _controller.GetAllowedDirectories());
    }
    
    [Fact]
    public void AddAllowedDirectory_WrongDirectory_PathNotAdded()
    {
        Assert.Throws<DirectoryNotFoundException>(() => _controller.AddAllowedDirectory(Path.Combine("D", "SomePath")));
        Assert.Empty(_controller.GetAllowedDirectories());
    }
}