using PluginInfrastructure.Network_Service;

namespace PluginInfrastructure.Tests;

public class NetworkPermissionControllerTests
{
    private readonly NetworkPermissionController _networkPermissionController = new NetworkPermissionController();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddAllowedUrl_WithEmptyUrl_ThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => _networkPermissionController.AddAllowedUrl(url));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("invalid://url")]
    [InlineData("http:/invalid.com")]
    public void AddAllowedUrl_WithInvalidUrl_ThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => _networkPermissionController.AddAllowedUrl(url));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://test.com/path")]
    [InlineData("https://subdomain.example.com/path?query=1")]
    public void AddAllowedUrl_WithValidUrl_AddsUrlToCollection(string url)
    {
        _networkPermissionController.AddAllowedUrl(url);
        
        var allowedUrls = _networkPermissionController.GetAllowedUrls();
        Assert.Single(allowedUrls);
        Assert.Contains(Normalizer.NormalizeUrl(url), allowedUrls);
    }
    
    [Theory]
    [InlineData("ftp://example.com")]
    [InlineData("file://localhost/c$/windows")]
    [InlineData("mailto:test@example.com")]
    public void AddAllowedUrl_WithUnsupportedScheme_ThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => _networkPermissionController.AddAllowedUrl(url));
    }

    [Fact]
    public void GetAllowedUrls_WithMultipleUrls_ReturnsAllUrls()
    {
        var urls = new[]
        {
            "https://example1.com",
            "https://example2.com",
            "https://example3.com"
        };
        
        foreach (var url in urls)
            _networkPermissionController.AddAllowedUrl(url);
        
        var allowedUrls = _networkPermissionController.GetAllowedUrls();
        Assert.Equal(urls.Length, allowedUrls.Count);
        foreach (var url in urls)
            Assert.Contains(Normalizer.NormalizeUrl(url), allowedUrls);
    }
}