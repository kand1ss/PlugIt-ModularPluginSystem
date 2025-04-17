namespace PluginAPI.Services.interfaces;

public interface IPluginNetworkService
{
    Task<byte[]> GetAsync(string url);
    Task<string> PostAsync(string url, HttpContent content);
}