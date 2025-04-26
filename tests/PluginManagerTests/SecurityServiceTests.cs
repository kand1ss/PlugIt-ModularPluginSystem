using System.Text.Json;
using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Services.Interfaces;
using Moq;
using Xunit;

namespace PluginManagerTests;

public class SecurityServiceTests : IDisposable
{
    private readonly SecurityService _securityService;
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "securityTests");

    public SecurityServiceTests()
    {
        var settingsProviderMock = new Mock<SecuritySettingsProvider>();
        var assemblySecurityMock = new Mock<IAssemblySecurityService>();
        var permissionSecurityMock = new Mock<IPluginPermissionSecurityService>();
        var loaderMock = new Mock<IAssemblyLoader>();
        var handlerMock = new Mock<IAssemblyHandler>();
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);

        _securityService = new SecurityService(
            settingsProviderMock.Object,
            assemblySecurityMock.Object,
            permissionSecurityMock.Object,
            loaderMock.Object,
            handlerMock.Object,
            logger);
        
        if(!Directory.Exists(_tempDirectory))
            Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void ImportJsonConfiguration_WithNonJsonExtension_ReturnsFalse()
    {
        string configPath = "config.txt";
        bool result = _securityService.ImportJsonConfiguration(configPath);

        Assert.False(result);
    }

    [Fact]
    public void ImportJsonConfiguration_WithValidJsonConfig_ReturnsTrue()
    {
        string configPath = Path.Combine(_tempDirectory, "config.json");
        var settings = new SecuritySettings();
        string json = JsonSerializer.Serialize(settings);

        File.WriteAllText(configPath, json);

        try
        {
            bool result = _securityService.ImportJsonConfiguration(configPath);
            Assert.True(result);
        }
        finally
        {
            if (File.Exists(configPath))
                File.Delete(configPath);
        }
    }

    [Fact]
    public void ImportJsonConfiguration_WithInvalidJsonConfig_ReturnsFalse()
    {
        string configPath = Path.Combine(_tempDirectory, "invalid_config.json");
        string invalidJson = "{invalid_json}";

        File.WriteAllText(configPath, invalidJson);

        try
        {
            bool result = _securityService.ImportJsonConfiguration(configPath);
            Assert.False(result);
        }
        finally
        {
            if (File.Exists(configPath))
                File.Delete(configPath);
        }
    }

    [Fact]
    public void ImportJsonConfiguration_WithFileNotFound_ThrowsFileNotFoundException()
    {
        string nonExistentPath = "non_existent_config.json";

        Assert.Throws<FileNotFoundException>(() => _securityService.ImportJsonConfiguration(nonExistentPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }
}