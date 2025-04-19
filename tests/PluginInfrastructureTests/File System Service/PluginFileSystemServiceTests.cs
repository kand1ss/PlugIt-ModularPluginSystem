using System.Text;
using Moq;
using PluginAPI.Services;
using PluginAPI.Services.interfaces;
using PluginInfrastructure;

namespace PluginManagerTests;

public class PluginFileSystemServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly IPluginFileSystemService _service;

    public PluginFileSystemServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_testDirectory);
        
        var controllerMock = new Mock<IFileSystemPermissionController>();
        controllerMock.Setup(x => x.GetAllowedDirectories())
            .Returns([Normalizer.NormalizeDirectoryPath(_testDirectory)]);

        _service = FileSystemServiceFactory.Create(controllerMock.Object);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
        catch
        {
        }
    }

    [Fact]
    public async Task Write_ValidPathAndData_DataWritten()
    {
        var testFile = Path.Combine(_testDirectory, "test.txt");
        var testData = Encoding.UTF8.GetBytes("Hello, World!");

        Assert.True(await _service.WriteAsync(testFile, testData));
        Assert.True(File.Exists(testFile));
        Assert.Equal(testData, await File.ReadAllBytesAsync(testFile));
    }

    [Fact]
    public async Task Write_OutsideAllowedDirectory_DataNotWritten()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        Assert.False(await _service.WriteAsync(testFile, testData));
        Assert.False(File.Exists(testFile));
    }

    [Fact]
    public async Task Write_PathTraversalAttempt_DataNotWritten()
    {
        var testFile = Path.Combine(_testDirectory, "..", "traversal.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        Assert.False(await _service.WriteAsync(testFile, testData));
        Assert.False(File.Exists(testFile));
    }

    [Fact]
    public async Task Read_ExistingFile_FileHasBeenRead()
    {
        var testFile = Path.Combine(_testDirectory, "read_test.txt");
        var testData = Encoding.UTF8.GetBytes("Test Content"); 
        
        await File.WriteAllBytesAsync(testFile, testData);
        Assert.Equal(testData, await _service.ReadAsync(testFile));
    }

    [Fact]
    public async Task Read_NonExistentFile_ShouldReturnEmptyArray()
    {
        var testFile = Path.Combine(_testDirectory, "non_existent.txt");
        Assert.Empty(await _service.ReadAsync(testFile));
    }

    [Fact]
    public async Task Read_OutsideAllowedDirectory_ShouldReturnEmptyArray()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        File.WriteAllBytes(testFile, Encoding.UTF8.GetBytes("Test"));

        Assert.Empty(await _service.ReadAsync(testFile));
        File.Delete(testFile);
    }

    [Fact]
    public async Task Write_InvalidPath_ShouldFail()
    {
        var testData = Encoding.UTF8.GetBytes("Test");
        Assert.False(await _service.WriteAsync("", testData));
    }
}