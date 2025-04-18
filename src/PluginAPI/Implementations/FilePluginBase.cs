using PluginAPI.Services.interfaces;
using PluginAPI.Stubs;

namespace PluginAPI;

public abstract class FilePluginBase : PluginBase, IFilePlugin
{
    private bool _isServiceInjected = false;
    protected IPluginFileSystemService FileSystemService { get; private set; } = new FileSystemServiceStub();
    
    public void InjectService(IPluginFileSystemService service)
        => FileSystemService = InjectService<IPluginFileSystemService>.TryInject(service, ref _isServiceInjected);
    
    public abstract Task WriteFileAsync(string path, byte[] data);
    public abstract Task<byte[]> ReadFileAsync(string path);
}