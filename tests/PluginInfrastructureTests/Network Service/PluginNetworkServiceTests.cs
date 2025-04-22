using System.Security;
using System.Text;
using PluginAPI.Models.Permissions;
using PluginAPI.Services.interfaces;
using PluginInfrastructure.Network_Service;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PluginInfrastructure.Tests;

public class PluginNetworkServiceTests : IDisposable
{
    private readonly string _getMethodUrl;
    private readonly string _postMethodUrl;
    private readonly WireMockServer _server;
    
    private static readonly string _postMethodResponse = "Response: Hello from WireMock!";
    private static readonly string _getMethodResponse = "Hello from WireMock!";

    private IPluginNetworkService _service;
    private readonly List<string> _allowedUrls = new() { "https://allowed.com", "https://permitted.com" };

    public PluginNetworkServiceTests()
    {
        _server = SetupHttpServer();

        _getMethodUrl = _server.Urls[0] + "/test";
        _postMethodUrl = _getMethodUrl + "/post";

        _allowedUrls.Add(_getMethodUrl);
        _allowedUrls.Add(_postMethodUrl);

        var controller = new NetworkPermissionController();
        foreach (var url in _allowedUrls)
            controller.AddAllowedUrl(url, new NetworkPermission());

        _service = new PluginNetworkService(controller);
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

        server.Given(
                Request.Create().WithPath("/test/error").UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(500)
            );

        server.Given(
                Request.Create().WithPath("/test/large").UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithBody(new byte[5 * 1024 * 1024]) // 5MB response
            );

        server.Given(
                Request.Create().WithPath("/test/retry").UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithFault(FaultType.EMPTY_RESPONSE)
            );

        return server;
    }

    [Fact]
    public async Task GetAsync_WithLargeResponse_ThrowsSecurityException()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_getMethodUrl + "/large", new NetworkPermission());
        _service = new PluginNetworkService(controller, 
            new NetworkServiceSettings { MaxResponseSizeMb = 1 });

        await Assert.ThrowsAsync<SecurityException>(
            async () => await _service.GetAsync(_getMethodUrl + "/large"));
    }

    [Fact]
    public async Task GetAsync_WithRetryableError_RetriesRequest()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_getMethodUrl + "/retry", new NetworkPermission());
        _service = new PluginNetworkService(controller,
            new NetworkServiceSettings { MaxRequestRetriesCount = 3 });

        var result = await _service.GetAsync(_getMethodUrl + "/retry");
        Assert.Empty(result);
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
        Assert.Equal(_getMethodResponse, Encoding.UTF8.GetString(response));
    }

    [Fact]
    public async Task GetAsync_WithAllowedUrlButWithoutPermission_ThrowsSecurityException()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_getMethodUrl, new NetworkPermission(false));
        _service = new PluginNetworkService(controller);
        
        await Assert.ThrowsAsync<SecurityException>(async () => await _service.GetAsync(_getMethodUrl));
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
        _service = new PluginNetworkService(controller);
        
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

    [Fact]
    public async Task PostAsync_WithCustomTimeout_TimesOut()
    {
        var controller = new NetworkPermissionController();
        controller.AddAllowedUrl(_postMethodUrl, new NetworkPermission());
        _service = new PluginNetworkService(controller,
            new NetworkServiceSettings { RequestTimeout = TimeSpan.FromMicroseconds(1) });

        await Assert.ThrowsAsync<TaskCanceledException>(
            async () => await _service.PostAsync(_postMethodUrl, new StringContent("test")));
    }

    public void Dispose()
    {
        _server?.Dispose();
    }
}