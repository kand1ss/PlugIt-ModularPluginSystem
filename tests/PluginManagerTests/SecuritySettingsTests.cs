using ModularPluginAPI.Components;
using PluginAPI.Models.Permissions;
using PluginInfrastructure;
using Xunit;

namespace PluginManagerTests;

public class SecuritySettingsTests
{
    private readonly SecuritySettings _securitySettings = new();
    
    private readonly FileSystemPermission _filePermission = new(Path.Combine("D", "SomePath"));
    private readonly NetworkPermission _networkPermission = new("http://localhost");
    
    [Fact]
    public void AddFileSystemPermission_CorrectPath_PermissionAdded()
    {
        _securitySettings.AddFileSystemPermission(_filePermission);
        
        Assert.Contains(Normalizer.NormalizeDirectoryPath(_filePermission.Path), 
            _securitySettings.FileSystemPermissions);
        Assert.Single(_securitySettings.FileSystemPermissions);
    }

    [Fact]
    public void AddFileSystemPermission_AlreadyAdded_DuplicatePermissionNotAdded()
    {
        _securitySettings.AddFileSystemPermission(_filePermission);
        _securitySettings.AddFileSystemPermission(_filePermission);

        Assert.Single(_securitySettings.FileSystemPermissions);
    }

    [Fact]
    public void AddNetworkPermission_CorrectUrl_PermissionAdded()
    {
        _securitySettings.AddNetworkPermission(_networkPermission);
        
        Assert.Contains(Normalizer.NormalizeUrl(_networkPermission.Path), _securitySettings.NetworkPermissions);
        Assert.Single(_securitySettings.NetworkPermissions);
    }

    [Theory]
    [InlineData("htt://localhost")]
    [InlineData("http:/localhost")]
    [InlineData("http//localhost")]
    public void AddNetworkPermission_IncorrectUrl_PermissionNotAdded(string url)
    {
        Assert.Throws<ArgumentException>(() => _securitySettings.AddNetworkPermission(new NetworkPermission(url)));
        Assert.Empty(_securitySettings.NetworkPermissions);
    }

    [Fact]
    public void AddNetworkPermission_AlreadyAdded_DuplicatePermissionNotAdded()
    {
        _securitySettings.AddNetworkPermission(_networkPermission);
        _securitySettings.AddNetworkPermission(_networkPermission);
        
        Assert.Single(_securitySettings.NetworkPermissions);
    }
}