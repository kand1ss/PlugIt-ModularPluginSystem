using System.Text;
using Moq;
using PluginAPI.Services;
using PluginAPI.Services.interfaces;

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
            .Returns(new[] { _testDirectory });

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
    public void Write_ValidPathAndData_DataWritten()
    {
        var testFile = Path.Combine(_testDirectory, "test.txt");
        var testData = Encoding.UTF8.GetBytes("Hello, World!");

        Assert.True(_service.Write(testFile, testData));
        Assert.True(File.Exists(testFile));
        Assert.Equal(testData, File.ReadAllBytes(testFile));
    }

    [Fact]
    public void Write_OutsideAllowedDirectory_DataNotWritten()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        Assert.False(_service.Write(testFile, testData));
        Assert.False(File.Exists(testFile));
    }

    [Fact]
    public void Write_PathTraversalAttempt_DataNotWritten()
    {
        var testFile = Path.Combine(_testDirectory, "..", "traversal.txt");
        var testData = Encoding.UTF8.GetBytes("Test");

        Assert.False(_service.Write(testFile, testData));
        Assert.False(File.Exists(testFile));
    }

    [Fact]
    public void Read_ExistingFile_FileHasBeenRead()
    {
        var testFile = Path.Combine(_testDirectory, "read_test.txt");
        var testData = Encoding.UTF8.GetBytes("Test Content");
        File.WriteAllBytes(testFile, testData);

        Assert.Equal(testData, _service.Read(testFile));
    }

    [Fact]
    public void Read_NonExistentFile_ShouldReturnEmptyArray()
    {
        var testFile = Path.Combine(_testDirectory, "non_existent.txt");
        Assert.Empty(_service.Read(testFile));
    }

    [Fact]
    public void Read_OutsideAllowedDirectory_ShouldReturnEmptyArray()
    {
        var testFile = Path.Combine(Path.GetTempPath(), "unauthorized.txt");
        File.WriteAllBytes(testFile, Encoding.UTF8.GetBytes("Test"));

        Assert.Empty(_service.Read(testFile));
        File.Delete(testFile);
    }

    [Fact]
    public void Write_InvalidPath_ShouldFail()
    {
        var testData = Encoding.UTF8.GetBytes("Test");
        Assert.False(_service.Write("", testData));
    }

    [Fact]
    public void Write_NullData_ShouldFail()
    {
        var testFile = Path.Combine(_testDirectory, "null_data.txt");
        Assert.False(_service.Write(testFile, null));
    }
}