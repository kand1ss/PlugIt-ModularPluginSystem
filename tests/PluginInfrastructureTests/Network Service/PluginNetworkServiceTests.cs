using System.Security;
using System.Text;
using PluginAPI.Models.Permissions;
using PluginAPI.Services.interfaces;
using PluginInfrastructure.Network_Service;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PluginInfrastructure.Tests;

public class PluginNetworkServiceTests
{
    private readonly string _getMethodUrl;
    private readonly string _postMethodUrl;
    
    private static readonly string _postMethodResponse = "Response: Hello from WireMock!";
    private static readonly string _getMethodResponse = "Hello from WireMock!";

    private IPluginNetworkService _service;
    private readonly List<string> _allowedUrls = new() { "https://allowed.com", "https://permitted.com" };

    public PluginNetworkServiceTests()
    {
        var server = SetupHttpServer();

        _getMethodUrl = server.Urls[0] + "/test";
        _postMethodUrl = _getMethodUrl + "/post";

        _allowedUrls.Add(_getMethodUrl);
        _allowedUrls.Add(_postMethodUrl);

        var controller = new NetworkPermissionController();
        foreach (var url in _allowedUrls)
            controller.AddAllowedUrl(url, new NetworkPermission());

        _service = PluginNetworkServiceFactory.Create(controller);
    }

    private static WireMockServer SetupHttpServer()
    {
        var server = WireMockServer.Start();
        
        server.Given(
                Request.Create().WithPath("/test").UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(_getMethodResponse)
            );

        server.Given(
                Request.Create().WithPath("/test/post").UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(_postMethodResponse)
            );
        return server;
    }

    [Fact]
    public async Task GetAsync_WithNotAllowedUrl_ThrowsSecurityException()
    {
        var notAllowedUrl = "https://notallowed.com";
        await Assert.ThrowsAsync<SecurityException>(async () => await _service.GetAsync(notAllowedUrl));
    }

    [Fact]
    public async Task GetAsync_WithAllowedUrl_CallsHttpClient()
    {
        var response = await _service.GetAsync(_getMethodUrl);
        Assert.NotNull(response);
        Assert.Equal(_getMethodResponse, Encoding.UTF8.GetString(response));;
    }

    [Fact]
    public async Task GetAsync_WithAllowedUrlButWithoutPermission_ThrowsSecurityException()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_getMethodUrl, new NetworkPermission(false));
        _service = PluginNetworkServiceFactory.Create(controller);
        
        await Assert.ThrowsAsync<SecurityException>(async () => await _service.GetAsync(_getMethodUrl));;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAsync_WithInvalidUrl_ThrowsArgumentException(string invalidUrl)
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetAsync(invalidUrl));
    }

    [Fact]
    public async Task PostAsync_WithNotAllowedUrl_ThrowsSecurityException()
    {
        var notAllowedUrl = "https://notallowed.com";
        var content = new StringContent("test");

        await Assert.ThrowsAsync<SecurityException>(async () => await _service.PostAsync(notAllowedUrl, content));
    }

    [Fact]
    public async Task PostAsync_WithAllowedUrl_CallsHttpClient()
    {
        var content = new StringContent("test");
        var result = await _service.PostAsync(_postMethodUrl, content);

        Assert.NotNull(result);
        Assert.Equal(_postMethodResponse, result);
    }
    
    [Fact]
    public async Task PostAsync_WithAllowedUrlButhWithoutPermission_ThrowsSecurityException()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_postMethodUrl, new NetworkPermission(true, false));
        _service = PluginNetworkServiceFactory.Create(controller);
        
        var content = new StringContent("test");
        await Assert.ThrowsAsync<SecurityException>(async () => await _service.PostAsync(_postMethodUrl, content));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task PostAsync_WithInvalidUrl_ThrowsArgumentException(string invalidUrl)
    {
        var content = new StringContent("test");
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.PostAsync(invalidUrl, content));
    }
}