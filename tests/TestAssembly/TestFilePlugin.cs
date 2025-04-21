using PluginAPI;

namespace TestAssembly;

public class TestFilePlugin : FilePluginBase
{
    public override string Name { get; } = string.Empty;
    public override Version Version { get; } = new(1, 0, 0);
    public override string Description { get; } = string.Empty;
    public override string Author { get; } = string.Empty;

    public bool IsRead { get; private set; } = false;
    public bool IsWrite { get; private set; } = false;

    public override Task WriteFileAsync(byte[] data)
    {
        IsWrite = true;
        return Task.CompletedTask;
    }

    public override Task<byte[]> ReadFileAsync()
    {
        IsRead = true;
        return Task.FromResult(Array.Empty<byte>());
    }
}