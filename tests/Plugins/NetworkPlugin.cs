using PluginAPI;

namespace Plugins;

public class NetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin";
    public override Version Version => new(1, 0, 1);
    public override string Author => "kand2s";


    public async Task<string> SendDataAsync(byte[] data)
    {
        Console.WriteLine($"Sending data: {data.Length} bytes");
        GetDependencyPlugin<IExecutablePlugin>("ConsolePlugin2").Execute();
        await Task.Yield();
        return "";
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        Console.WriteLine("Receiving data");
        await Task.Yield();
        return [];
    }
}