namespace PluginAPI;

public interface IFilePlugin : IPlugin
{
    Task WriteFileAsync(string path, byte[] data);
    Task<byte[]> ReadFileAsync(string path);
}