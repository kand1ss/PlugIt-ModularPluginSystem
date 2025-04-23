using System.Security;
using PluginAPI.Models.Permissions;
using PluginAPI.Models.Permissions.Interfaces;
using PluginAPI.Services.interfaces;

namespace PluginInfrastructure.Network_Service;

public class PluginNetworkService : IPluginNetworkService, IDisposable
{
    private readonly Dictionary<string, NetworkPermission> _allowedPermissions = new();
    private readonly HttpClientHandler _httpClientHandler = new();
    private readonly HttpClient _httpClient;
    
    private readonly long _maxResponseSize;
    private readonly int _maxRetries;
    
    private readonly NetworkPermissionChecker _permissionChecker;

    public PluginNetworkService(IPermissionController<NetworkPermission> controller, NetworkServiceSettings? settings = null)
    {
        settings ??= new();
        foreach(var url in controller.GetPermissions())
            _allowedPermissions.Add(url.Path, url);

        _maxResponseSize = settings.MaxResponseSizeBytes;
        _httpClientHandler.MaxAutomaticRedirections = settings.MaxRedirectionsCount;
        if (settings.MaxRedirectionsCount != 0)
            _httpClientHandler.AllowAutoRedirect = true;
        
        _httpClient = new(_httpClientHandler);
        _httpClient.Timeout = settings.RequestTimeout;
        _maxRetries = settings.MaxRequestRetriesCount;
        _permissionChecker = new(_allowedPermissions);
    }

    private void CheckUrlAllowed(string url, bool isGet)
    {
        url = Normalizer.NormalizeUrl(url);
        if (!_permissionChecker.CheckPermissionAllow(url, out var permission) || permission == null)
            throw new SecurityException($"URL '{url}' is not permitted.");
        
        if (isGet && !permission.CanGet)
            throw new SecurityException($"URL '{url}' does not allow GET requests.");
        if (!isGet && !permission.CanPost)
            throw new SecurityException($"URL '{url}' does not allow POST requests.");
    }
    
    public async Task<string> PostAsync(string url, HttpContent content)
        => await RetryWrap(async () => await TryPostAsync(url, content)) ?? "";

    private async Task<TResult?> RetryWrap<TResult>(Func<Task<TResult>> func)
    {
        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return await func();
            }
            catch (HttpRequestException) when (attempt < _maxRetries)
            {
                await Task.Delay(1000);
            }
            catch (HttpRequestException)
            {
                break;
            }
        }
        
        return default;
    }
    
    private async Task<string> TryPostAsync(string url, HttpContent content)
    {
        CheckUrlAllowed(url, false);
        var response = await _httpClient.PostAsync(url, content);

        await ValidateResponseAsync(response);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<byte[]> GetAsync(string url)
        => await RetryWrap(async () => await TryGetAsync(url)) ?? [];
    
    private async Task<byte[]> TryGetAsync(string url)
    {
        CheckUrlAllowed(url, true);
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        return await ValidateResponseAsync(response);
    }

    private async Task<byte[]> ValidateResponseAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        
        var contentLength = response.Content.Headers.ContentLength;
        if (contentLength.HasValue)
            CheckContentSize(contentLength.Value);
        else
        {
            var bytes = await CreateAndReadStream(response);
            return bytes;
        }
        
        return await response.Content.ReadAsByteArrayAsync();
    }
    
    private void CheckContentSize(long contentLength)
    {
        if (contentLength > _maxResponseSize)
            throw new SecurityException(
                $"Response size '{contentLength}' exceeds the allowable size '{_maxResponseSize}'.");
    }

    private async Task<byte[]> CreateAndReadStream(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        var buffer = new byte[8192];
        var totalBytesRead = 0;
        var memoryStream = new MemoryStream();

        int bytesRead;
        
        while((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            totalBytesRead += bytesRead;
            CheckContentSize(totalBytesRead);
                
            await memoryStream.WriteAsync(buffer, 0, bytesRead);
        }

        return memoryStream.ToArray();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}