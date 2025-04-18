using PluginAPI.Services.interfaces;

namespace PluginAPI.Stubs;

public class FileSystemServiceStub : IPluginFileSystemService
{
    public Task<bool> WriteAsync(string absolutePath, byte[] dataToWrite)
        => throw new InvalidOperationException("File system service is not implemented");

    public Task<byte[]> ReadAsync(string absolutePath)
        => throw new InvalidOperationException("File system service is not implemented");
}