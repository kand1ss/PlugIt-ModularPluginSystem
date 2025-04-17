using PluginAPI;

namespace Plugins;

public class NetworkPlugin : PluginBase, INetworkPlugin
{
    public override string Name => "NetworkPlugin";
    public override Version Version => new(1, 0, 1);
    public override string Description => "Console plugin";
    public override string Author => "kand2s";


    public Task SendDataAsync(byte[] data)
    {
        Console.WriteLine($"Sending data: {data.Length} bytes");
        GetDependencyPlugin<IExecutablePlugin>("ConsolePlugin2").Execute();
        return Task.CompletedTask;
    }

    public async Task<byte[]> ReceiveDataAsync()
    {
        Console.WriteLine("Receiving data");
        await Task.Yield();
        return [];
    }
}