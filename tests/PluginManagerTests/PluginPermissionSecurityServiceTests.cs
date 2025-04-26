using ModularPluginAPI.Components;
using ModularPluginAPI.Models;
using PluginAPI.Models.Permissions;
using Xunit;

namespace PluginManagerTests;

public class PluginPermissionSecurityServiceTests
{
    private readonly PluginPermissionSecurityService _securityService;
    private readonly SecuritySettingsProvider _settingsProvider = new();
    private SecuritySettings _settings => _settingsProvider.Settings;
    
    private readonly PluginMetadata _metadata = new();
    
    private readonly string _path = Path.Combine("D", "SomePath");
    private readonly string _path1 = Path.Combine("D", "SomePath2");
    private readonly string _path2 = Path.Combine("D", "SomePath3");

    private readonly string _url = "https://localhost";
    private readonly string _url1 = "http://localhost";

    
    public PluginPermissionSecurityServiceTests()
    {
        _securityService = new(_settingsProvider);
    }

    [Fact]
    public void CheckSafety_SafetyFileSystemPermissions_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path2);
        _settings.AddFileSystemPermission(new FileSystemPermission(_path));
        _settings.AddFileSystemPermission(new FileSystemPermission(_path2));
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_SafetyNetworkPermissions_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url1);
        _settings.AddNetworkPermission(new NetworkPermission(_url));;
        _settings.AddNetworkPermission(new NetworkPermission(_url1));;
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_SafetyBothPermissions_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url1);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path1);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path2);
        
        _settings.AddFileSystemPermission(new FileSystemPermission(_path));
        _settings.AddFileSystemPermission(new FileSystemPermission(_path1));
        _settings.AddFileSystemPermission(new FileSystemPermission(_path2));
        _settings.AddNetworkPermission(new NetworkPermission(_url));
        _settings.AddNetworkPermission(new NetworkPermission(_url1));
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_NetworkPermissionNotSafety_ReturnsFalse()
    {
        _settings.AddFileSystemPermission(new FileSystemPermission(_path));
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        
        Assert.False(_securityService.CheckSafety(_metadata));
    }
    
    [Fact]
    public void CheckSafety_FilePermissionNotSafety_ReturnsFalse()
    {
        _settings.AddNetworkPermission(new NetworkPermission(_url));;
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        
        Assert.False(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_EmptyPluginConfiguration_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.FileSystemPaths.Clear();
        _metadata.Configuration.Permissions.NetworkURLs.Clear();
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }
}