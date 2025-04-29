using PluginInfrastructure.Normalization.Mappers;

namespace PluginInfrastructureTests;

public class CrossPlatformPathMapperTests
{
    private readonly CrossPlatformPathMapper _mapper = new();

    [Theory]
    [InlineData("/home/user/documents", "C:\\Users\\user\\documents")]
    [InlineData("/home/admin", "C:\\Users\\admin")]
    [InlineData("/root/config", "C:\\Administrator\\config")]
    [InlineData("/usr/bin/app", "C:\\Program Files\\app")]
    [InlineData("/tmp/file.txt", $"C:\\Temp\\file.txt")]
    [InlineData("/var/log", "C:\\var\\log")]
    public void MapToWindows_WithUnixPaths_MapsCorrectly(string unixPath, string expectedWindowsPath)
    {
        var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        var expectedPath = expectedWindowsPath.Replace("C:", systemDrive);

        var result = _mapper.MapToWindows(unixPath);

        Assert.Equal(expectedPath, result);
    }

    [Theory]
    [InlineData("C:\\Users\\user\\documents", "/home/user/documents")]
    [InlineData("C:\\Users\\admin", "/home/admin")]
    [InlineData("C:\\Administrator\\config", "/root/config")]
    [InlineData("C:\\Program Files\\app", "/usr/bin/app")]
    [InlineData("C:\\Program Files (x86)\\app", "/usr/bin/app")]
    [InlineData($"C:\\Temp\\file.txt", "/tmp/file.txt")]
    [InlineData("D:\\Data", "/Data")]
    public void MapToUnix_WithWindowsPaths_MapsCorrectly(string windowsPath, string expectedUnixPath)
    {
        var result = _mapper.MapToUnix(windowsPath);
        Assert.Equal(expectedUnixPath, result);
    }

    [Fact]
    public void MapToWindows_WithNonUnixPath_ReturnsOriginalPath()
    {
        var nonUnixPath = "C:\\Users\\user";
        var result = _mapper.MapToWindows(nonUnixPath);

        Assert.Equal(nonUnixPath, result);
    }

    [Fact]
    public void MapToWindows_WithAlreadyWindowsPath_ReturnsOriginalPath()
    {
        var windowsPath = "C:\\Users\\user";
        var result = _mapper.MapToWindows(windowsPath);

        Assert.Equal(windowsPath, result);
    }

    [Fact]
    public void MapToUnix_WithNonWindowsPath_ReturnsOriginalPath()
    {
        var nonWindowsPath = "/home/user";
        var result = _mapper.MapToUnix(nonWindowsPath);

        Assert.Equal(nonWindowsPath, result);
    }

    [Fact]
    public void MapToUnix_WithAlreadyUnixPath_ReturnsOriginalPath()
    {
        var unixPath = "/home/user";
        var result = _mapper.MapToUnix(unixPath);

        Assert.Equal(unixPath, result);
    }

    [Fact]
    public void MapToCurrentOS_OnWindows_CallsMapToWindows()
    {
        var unixPath = "/home/user/documents";
        var windowsPath = "C:\\Users\\user\\documents";
        var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        var expectedPath = windowsPath.Replace("C:", systemDrive);

        if (OperatingSystem.IsWindows())
        {
            var result = _mapper.MapToCurrentOS(unixPath);
            Assert.Equal(expectedPath, result);
        }
        else
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void MapToCurrentOS_OnLinux_CallsMapToUnix()
    {
        var windowsPath = "C:\\Users\\user\\documents";
        var unixPath = "/home/user/documents";

        if (OperatingSystem.IsLinux())
        {
            var result = _mapper.MapToCurrentOS(windowsPath);
            Assert.Equal(unixPath, result);
        }
        else
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void MapToWindows_WithComplexUnixPath_MapsCorrectly()
    {
        var unixPath = "/home/user/documents/usr/bin/app/config";
        var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        var expectedPath = $"{systemDrive}\\Users\\user\\documents\\Program Files\\app\\config";

        var result = _mapper.MapToWindows(unixPath);

        Assert.Equal(expectedPath, result);
    }

    [Fact]
    public void MapToUnix_WithComplexWindowsPath_MapsCorrectly()
    {
        var windowsPath = "C:\\Users\\user\\documents\\Program Files\\app\\config";
        var expectedPath = "/home/user/documents/usr/bin/app/config";

        var result = _mapper.MapToUnix(windowsPath);

        Assert.Equal(expectedPath, result);
    }

    [Fact]
    public void MapToWindows_WithEmptyPath_ReturnsOriginalPath()
    {
        var result = _mapper.MapToWindows("");
        Assert.Equal("", result);
    }

    [Fact]
    public void MapToUnix_WithEmptyPath_ReturnsOriginalPath()
    {
        var result = _mapper.MapToUnix("");
        Assert.Equal("", result);
    }
}