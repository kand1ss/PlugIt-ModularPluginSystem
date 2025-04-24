using PluginAPI.Models.Permissions;
using PluginInfrastructure;
using PluginInfrastructure.Network_Service;

namespace PluginInfrastructureTests.Checkers;

public class NetworkPermissionCheckerTests
    {
        [Fact]
        public void CheckPermissionAllow_WithExistingUrl_ReturnsTrue()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com") }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            
            bool result = checker.CheckPermissionExists("https://example.com", out var permission);
            
            Assert.True(result);
            Assert.NotNull(permission);
            Assert.Equal("https://example.com", permission.Path);
        }
        
        [Fact]
        public void CheckPermissionAllow_WithNonExistingUrl_ReturnsFalse()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com") }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            bool result = checker.CheckPermissionExists("https://othersite.com", out var permission);
            
            Assert.False(result);
            Assert.Null(permission);
        }
        
        [Fact]
        public void CheckPermissionAllow_UrlCaseInsensitive_ReturnsTrue()
        {
            var normalizedUrl = Normalizer.NormalizeUrl("https://Example.Com");
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { normalizedUrl, new NetworkPermission(normalizedUrl) }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            bool result = checker.CheckPermissionExists("https://example.com", out var permission);
            
            Assert.True(result);
            Assert.NotNull(permission);
        }
        
        [Fact]
        public void CheckPermissionAllow_NormalizesInputUrl_ReturnsTrue()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com") }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            bool result = checker.CheckPermissionExists("https://Example.Com/", out var permission);
            
            Assert.True(result);
            Assert.NotNull(permission);
        }
        
        [Fact]
        public void CheckPermissionAllow_WithSpecificPermissions_ReturnsCorrectPermission()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com", canGet: true, canPost: false) },
                { "https://api.example.com", new NetworkPermission("https://api.example.com", canGet: false, canPost: true) }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            bool result = checker.CheckPermissionExists("https://api.example.com", out var permission);
            
            Assert.True(result);
            Assert.NotNull(permission);
            Assert.False(permission.CanGet);
            Assert.True(permission.CanPost);
        }
        
        [Fact]
        public void CheckPermissionsAllow_AllUrlsExist_ReturnsTrue()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com") },
                { "https://api.example.com", new NetworkPermission("https://api.example.com") }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            
            bool result = checker.CheckPermissionsExists(new List<string> 
            { 
                "https://example.com", 
                "https://api.example.com" 
            });
            
            Assert.True(result);
        }
        
        [Fact]
        public void CheckPermissionsAllow_OneUrlDoesNotExist_ReturnsFalse()
        {
            var permissions = new Dictionary<string, NetworkPermission>
            {
                { "https://example.com", new NetworkPermission("https://example.com") }
            };
            
            var checker = new NetworkPermissionChecker(permissions);
            
            bool result = checker.CheckPermissionsExists(new List<string> 
            { 
                "https://example.com", 
                "https://api.example.com" 
            });
            
            Assert.False(result);
        }
        
        [Fact]
        public void CheckPermissionAllow_WithInvalidUrl_ThrowsArgumentException()
        {
            var permissions = new Dictionary<string, NetworkPermission>();
            var checker = new NetworkPermissionChecker(permissions);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                checker.CheckPermissionExists("invalid-url", out var permission));
        }
    }