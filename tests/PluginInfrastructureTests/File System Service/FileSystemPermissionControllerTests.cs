using PluginAPI.Services;
using PluginInfrastructure;

namespace PluginManagerTests;

public class FileSystemPermissionControllerTests
{
    private readonly FileSystemPermissionController _controller = new();

    [Fact]
    public void AddAllowedDirectory_CorrectDirectory_NormalizedPathAdded()
    {
        var path = Directory.GetCurrentDirectory();
        _controller.AddAllowedDirectory(path);

        Assert.Single(_controller.GetAllowedDirectories());
        Assert.Contains(Normalizer.NormalizeDirectoryPath(path), _controller.GetAllowedDirectories());
    }
}