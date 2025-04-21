namespace PluginAPI;

public interface IFilePlugin : IConfigurablePlugin
{
    Task WriteFileAsync(byte[] data);
    Task<byte[]> ReadFileAsync();
}