using PluginAPI.Models.Permissions;
using PluginAPI.Services;
using PluginInfrastructure;

namespace PluginInfrastructureTests.Checkers;

public class FileSystemPermissionCheckerTests
{
    private readonly string _testDirectory = Normalizer.NormalizeDirectoryPath(Path.Combine("C", "TestDirectory"));
    private readonly string _testRecursiveDirectory = Normalizer.NormalizeDirectoryPath(Path.Combine("C", "TestRecursive")); 
    private readonly string _readOnlyDirectory = Normalizer.NormalizeDirectoryPath(Path.Combine("C", "ReadOnly"));

    private readonly FileSystemPermissionChecker _checker;

    public FileSystemPermissionCheckerTests()
    {
        Dictionary<string, FileSystemPermission> permissions = new()
        {
            { _testDirectory, new FileSystemPermission(_testDirectory, true, true, false) },
            { _testRecursiveDirectory, new FileSystemPermission(_testRecursiveDirectory, true, true, true) },
            { _readOnlyDirectory, new FileSystemPermission(_readOnlyDirectory, true, false, false) }
        };
        _checker = new FileSystemPermissionChecker(permissions);
    }

    [Fact]
    public void CheckPermissionAllow_WithExactPath_ShouldReturnTrueAndPermission()
    {
        var result = _checker.CheckPermissionExists(_testDirectory, out var permission);

        Assert.True(result);
        Assert.NotNull(permission);
        Assert.Equal(_testDirectory, permission.Path);
        Assert.False(permission.Recursive);
    }

    [Fact]
    public void CheckPermissionAllow_WithChildPath_AndNonRecursiveParent_ShouldReturnFalse()
    {
        var path = Path.Combine(_testDirectory, "SubFolder");
        var result = _checker.CheckPermissionExists(path, out var permission);

        Assert.False(result);
        Assert.Null(permission);
    }

    [Theory]
    [InlineData("SubFolder")]
    [InlineData("SubFolder/DeepFolder")]
    [InlineData("file.txt")]
    public void CheckPermissionAllow_WithChildPathAndRecursiveParent_ShouldReturnTrueAndPermission(string relativePath)
    {
        var path = Normalizer.NormalizeDirectoryPath(Path.Combine(_testRecursiveDirectory, relativePath));
        var result = _checker.CheckPermissionExists(path, out var permission);

        Assert.True(result);
        Assert.NotNull(permission);
        Assert.Equal(_testRecursiveDirectory, permission.Path);
        Assert.True(permission.Recursive);
    }

    [Fact]
    public void CheckPermissionAllow_WithParentDirectory_ShouldReturnTrueAndPermission()
    {
        var path = Path.Combine(_testDirectory, "file.txt");
        var result = _checker.CheckPermissionExists(path, out var permission);

        Assert.True(result);
        Assert.NotNull(permission);
        Assert.Equal(_testDirectory, permission.Path);
    }

    [Fact]
    public void CheckPermissionAllow_WithUnknownPath_ShouldReturnFalse()
    {
        var path = Path.Combine("D:", "UnknownPath");
        var result = _checker.CheckPermissionExists(path, out var permission);

        Assert.False(result);
        Assert.Null(permission);
    }

    [Fact]
    public void CheckPermissionAllow_WithMixedCasePath_ShouldIgnoreCaseOnWindows()
    {
        if (!OperatingSystem.IsWindows())
            return;
        
        var mixedCasePath = _testDirectory.ToUpper();
        var result = _checker.CheckPermissionExists(mixedCasePath, out var permission);

        Assert.True(result);
        Assert.NotNull(permission);
        Assert.Equal(_testDirectory, permission.Path);
    }

    [Fact]
    public void CheckPermissionAllow_WithTrailingSlash_ShouldNormalizeAndMatch()
    {
        var pathWithSlash = _testDirectory + Path.DirectorySeparatorChar;
        var result = _checker.CheckPermissionExists(pathWithSlash, out var permission);

        Assert.True(result);
        Assert.NotNull(permission);
        Assert.Equal(_testDirectory, permission.Path);
    }
}