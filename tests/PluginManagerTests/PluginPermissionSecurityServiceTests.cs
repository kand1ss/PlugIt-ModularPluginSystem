using ModularPluginAPI.Components;
using ModularPluginAPI.Models;
using Xunit;

namespace PluginManagerTests;

public class PluginPermissionSecurityServiceTests
{
    private readonly PluginPermissionSecurityService _securityService = new();
    private readonly PluginMetadata _metadata = new();
    
    private readonly string _path = Path.Combine("D", "SomePath");
    private readonly string _path1 = Path.Combine("D", "SomePath");
    private readonly string _path2 = Path.Combine("D", "SomePath");

    private readonly string _url = "https://localhost";
    private readonly string _url1 = "http://localhost";

    [Fact]
    public void AddFileSystemPermission_CorrectPath_PermissionAdded()
    {
        _securityService.AddFileSystemPermission(_path);
        
        Assert.Contains(_path, _securityService.GetFileSystemPermissions());
        Assert.Single(_securityService.GetFileSystemPermissions());
    }

    [Fact]
    public void AddFileSystemPermission_AlreadyAdded_DuplicatePermissionNotAdded()
    {
        _securityService.AddFileSystemPermission(_path);
        _securityService.AddFileSystemPermission(_path);
        
        Assert.Single(_securityService.GetFileSystemPermissions());
    }

    [Fact]
    public void AddNetworkPermission_CorrectUrl_PermissionAdded()
    {
        _securityService.AddNetworkPermission(_url);
        
        Assert.Contains(_url, _securityService.GetNetworkPermissions());
        Assert.Single(_securityService.GetNetworkPermissions());
    }

    [Fact]
    public void AddNetworkPermission_IncorrectUrl_PermissionNotAdded()
    {
        var url = "htt://localhost";
        var url2 = "http:/localhost";
        var url3 = "http//localhost";
        
        _securityService.AddNetworkPermission(url);
        _securityService.AddNetworkPermission(url2);
        _securityService.AddNetworkPermission(url3);
        
        Assert.Empty(_securityService.GetNetworkPermissions());
    }

    [Fact]
    public void AddNetworkPermission_AlreadyAdded_DuplicatePermissionNotAdded()
    {
        _securityService.AddNetworkPermission(_url);
        _securityService.AddNetworkPermission(_url);
        
        Assert.Single(_securityService.GetNetworkPermissions());
    }

    [Fact]
    public void CheckSafety_SafetyFileSystemPermissions_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path2);
        _securityService.AddFileSystemPermission(_path);
        _securityService.AddFileSystemPermission(_path2);
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_SafetyNetworkPermissions_ReturnsTrue()
    {
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url1);
        _securityService.AddNetworkPermission(_url);
        _securityService.AddNetworkPermission(_url1);
        
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
        
        _securityService.AddFileSystemPermission(_path);
        _securityService.AddFileSystemPermission(_path1);
        _securityService.AddFileSystemPermission(_path2);
        _securityService.AddNetworkPermission(_url);
        _securityService.AddNetworkPermission(_url1);
        
        Assert.True(_securityService.CheckSafety(_metadata));
    }

    [Fact]
    public void CheckSafety_NetworkPermissionNotSafety_ReturnsFalse()
    {
        _securityService.AddFileSystemPermission(_path);
        _metadata.Configuration.Permissions.FileSystemPaths.Add(_path);
        _metadata.Configuration.Permissions.NetworkURLs.Add(_url);
        
        Assert.False(_securityService.CheckSafety(_metadata));
    }
    
    [Fact]
    public void CheckSafety_FilePermissionNotSafety_ReturnsFalse()
    {
        _securityService.AddNetworkPermission(_url);
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