using PluginAPI;

namespace TestServices;

public class NetworkPlugin : NetworkPluginBase
{
    public override string Name { get; } = "NetworkPlugin";
    public override Version Version { get; } = new(1, 0, 0);
    public override string Author { get; } = "NetworkPlugin";
    
    
    public override async Task<string> SendDataAsync(byte[] data)
    {
        await GetDependencyPlugin<IFilePlugin>("FilePlugin").WriteFileAsync(data);
        return "";
    }

    public override async Task<byte[]> ReceiveDataAsync()
        => await NetworkService.GetAsync("https://httpbin.org/get");
}