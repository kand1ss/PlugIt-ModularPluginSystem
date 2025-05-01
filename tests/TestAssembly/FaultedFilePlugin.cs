using PluginAPI;

namespace TestAssembly;

public class FaultedFilePlugin : FilePluginBase
{
    public override string Name { get; } = string.Empty;
    public override Version Version { get; } = new Version(1, 0, 0);
    public override string Author { get; } = string.Empty;
    
    
    public override Task WriteFileAsync(byte[] data)
    {
        throw new Exception();
    }

    public override Task<byte[]> ReadFileAsync()
    {
        throw new Exception();
    }
}