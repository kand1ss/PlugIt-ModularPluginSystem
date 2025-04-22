using System.Security;
using System.Text;
using Moq;
using PluginAPI.Models.Permissions;
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
            .Returns(new Dictionary<string, FileSystemPermission> 
            { 
                { Normalizer.NormalizeDirectoryPath(_testDirectory), new FileSystemPermission(true, true, false) } 
            });

        _service = new PluginFileSystemService(controllerMock.Object, new FileSystemServiceSettings() { MaxFileSizeMb = 10 });
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

        await _service.WriteAsync(testFile, testData);
        Assert.True(File.Exists(testFile));
        Assert.Equal(testData, await File.ReadAllBytesAsync(testFile));
    }

    [Fact]
    public async Task Write_OutsideAllowedDirectory_ThrowsSecurityException()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        await Assert.ThrowsAsync<SecurityException>(async() => await _service.WriteAsync(testFile, testData));
        Assert.False(File.Exists(testFile));
    }

    [Fact]
    public async Task Write_PathTraversalAttempt_ThrowsSecurityException()
    {
        var testFile = Path.Combine(_testDirectory, "..", "traversal.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        await Assert.ThrowsAsync<SecurityException>(async() => await _service.WriteAsync(testFile, testData));
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
    public async Task Read_OutsideAllowedDirectory_ThrowsSecurityException()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        await File.WriteAllBytesAsync(testFile, Encoding.UTF8.GetBytes("Test"));

        await Assert.ThrowsAsync<SecurityException>(async () => await _service.ReadAsync(testFile));
        File.Delete(testFile);
    }
    
    [Fact]
    public async Task Write_ExceedingSizeLimit_ThrowsSecurityException()
    {
        var testFile = Path.Combine(_testDirectory, "large.txt");
        var largeData = new byte[1024 * 1024 * 11];
    
        await Assert.ThrowsAsync<SecurityException>(() => _service.WriteAsync(testFile, largeData));
    }
    
    [Fact]
    public async Task Read_WithoutReadPermission_ThrowsSecurityException()
    {
        var controllerMock = new Mock<IFileSystemPermissionController>();
        controllerMock.Setup(x => x.GetAllowedDirectories())
            .Returns(new Dictionary<string, FileSystemPermission> 
            { 
                { Normalizer.NormalizeDirectoryPath(_testDirectory), new FileSystemPermission(false, true) } 
            });
        var service = new PluginFileSystemService(controllerMock.Object);
    
        var testFile = Path.Combine(_testDirectory, "test.txt");
        await File.WriteAllTextAsync(testFile, "Test");
    
        await Assert.ThrowsAsync<SecurityException>(() => service.ReadAsync(testFile));
    }
    
    [Fact]
    public async Task Read_ExceedingSizeLimit_ThrowsSecurityException()
    {
        var testFile = Path.Combine(_testDirectory, "large.txt");
        var largeData = new byte[1024 * 1024 * 11];
        await File.WriteAllBytesAsync(testFile, largeData);
    
        await Assert.ThrowsAsync<SecurityException>(() => _service.ReadAsync(testFile));
    }
}