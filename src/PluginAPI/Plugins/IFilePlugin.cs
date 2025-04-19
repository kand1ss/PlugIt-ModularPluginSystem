namespace PluginAPI;

public interface IFilePlugin : IConfigurablePlugin
{
    Task WriteFileAsync(string path, byte[] data);
    Task<byte[]> ReadFileAsync(string path);
}