using PluginAPI;

namespace TestServices;

public class FilePlugin : FilePluginBase
{
    public override string Name { get; } = "FilePlugin";
    public override Version Version { get; } = new(1,0,0);
    public override string Description { get; } = "FilePlugin";
    public override string Author { get; } = "FilePlugin";
    
    
    public override async Task WriteFileAsync(byte[] data)
        => await FileSystemService.WriteAsync(@"C:\Users\kand1s\Desktop\Target.txt", data);

    public override async Task<byte[]> ReadFileAsync()
        => await FileSystemService.ReadAsync(@"C:\Users\kand1s\Desktop\Target.txt");
}