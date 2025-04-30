using PluginInfrastructure.Normalization.Mappers;

namespace PluginInfrastructureTests;

public class CrossPlatformPathMapperTests
{
    private readonly CrossPlatformPathMapper _mapper = new();

    [Theory]
    [InlineData("/home/user/documents", "C:\\Users\\user\\documents")]
    [InlineData("/root/projects", "C:\\Users\\Administrator\\projects")]
    [InlineData("/usr/share/common/data", "C:\\Users\\Public\\data")]
    [InlineData("/.local/share/app", "C:\\AppData\\Local\\app")]
    [InlineData("/.config/settings", "C:\\AppData\\Roaming\\settings")]
    [InlineData("/etc/system", "C:\\ProgramData\\system")]
    [InlineData("/usr/bin/program", "C:\\Program Files\\program")]
    [InlineData("/.cache/tmp/file", "C:\\AppData\\Local\\Temp\\file")]
    public void MapToWindows_FromUnixPath_ReturnsCorrectWindowsPath(string unixPath, string expectedWindowsPath)
    {
        var expectedDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        var expected = expectedWindowsPath.Replace("C:", expectedDrive);
        
        var result = _mapper.MapToWindows(unixPath);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C:\\SomeFolder\\file.txt")]
    [InlineData("D:\\Data\\Projects\\")]
    public void MapToWindows_AlreadyWindowsPath_ReturnsSamePath(string windowsPath)
    {
        var result = _mapper.MapToWindows(windowsPath);
        Assert.Equal(windowsPath, result);
    }

    [Fact]
    public void MapToWindows_EmptyPath_ReturnsSamePath()
    {
        var result = _mapper.MapToWindows("");
        Assert.Equal("", result);
    }

    [Theory]
    [InlineData("C:\\Users\\user\\documents", "/home/user/documents")]
    [InlineData("C:\\Users\\Administrator\\projects", "/root/projects")]
    [InlineData("C:\\Users\\Public\\data", "/usr/share/common/data")]
    [InlineData("C:\\AppData\\Local\\app", "/.local/share/app")]
    [InlineData("C:\\AppData\\Roaming\\settings", "/.config/settings")]
    [InlineData("C:\\Users\\user\\AppData\\Roaming\\settings", "/home/user/.config/settings")]
    [InlineData("C:\\ProgramData\\system", "/etc/system")]
    [InlineData("C:\\Program Files\\program", "/usr/bin/program")]
    [InlineData("C:\\Program Files (x86)\\program", "/usr/bin/program")]
    [InlineData("C:\\AppData\\Local\\Temp\\file", "/.cache/tmp/file")]
    [InlineData("D:\\Users\\user\\documents", "/home/user/documents")]
    public void MapToUnix_FromWindowsPath_ReturnsCorrectUnixPath(string windowsPath, string expectedUnixPath)
    {
        var result = _mapper.MapToUnix(windowsPath);
        Assert.Equal(expectedUnixPath, result);
    }

    [Theory]
    [InlineData("/home/user")]
    [InlineData("/var/log")]
    public void MapToUnix_AlreadyUnixPath_ReturnsSamePath(string unixPath)
    {
        var result = _mapper.MapToUnix(unixPath);
        Assert.Equal(unixPath, result);
    }

    [Fact]
    public void MapToUnix_EmptyOrNullPath_ReturnsSamePath()
    {
        var result = _mapper.MapToUnix("");
        Assert.Equal("", result);
    }

    [Theory]
    [InlineData("C:\\Users\\Administrator\\AppData\\Local\\app", "/root/.local/share/app")]
    [InlineData("C:\\Users\\Public\\Program Files\\app", "/usr/share/common/usr/bin/app")]
    public void MapToUnix_ComplexPaths_ReturnsCorrectMapping(string windowsPath, string expectedUnixPath)
    {
        var result = _mapper.MapToUnix(windowsPath);
        Assert.Equal(expectedUnixPath, result);
    }

    [Theory]
    [InlineData("/root/.config/app", "C:\\Users\\Administrator\\AppData\\Roaming\\app")]
    [InlineData("/home/usr/bin/app", "C:\\Users\\Program Files\\app")]
    public void MapToWindows_ComplexPaths_ReturnsCorrectMapping(string unixPath, string expectedWindowsPath)
    {
        var expectedDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        var expected = expectedWindowsPath.Replace("C:", expectedDrive);
        
        var result = _mapper.MapToWindows(unixPath);
        Assert.Equal(expected, result);
    }
}