using System.Security;
using PluginAPI.Services.interfaces;
using PluginInfrastructure;

namespace PluginAPI.Services;

internal class PluginFileSystemService : IPluginFileSystemService
{
    private readonly HashSet<string> _allowedDirectories = new();
    private readonly long _maxFileSize;

    public PluginFileSystemService(IFileSystemPermissionController controller, FileSystemServiceSettings? settings = null)
    {
        settings ??= new();
        foreach (var directory in controller.GetAllowedDirectories())
            _allowedDirectories.Add(directory);
        
        _maxFileSize = settings.MaxFileSizeBytes;
    }

    private bool IsPathAllowed(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;
        
        var normalizedPath = Normalizer.NormalizeDirectoryPath(path);
        return _allowedDirectories.Any(dir => normalizedPath.StartsWith(dir, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> WriteAsync(string absolutePath, byte[] dataToWrite)
    {
        try
        {
            return await TryWrite(absolutePath, dataToWrite);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> TryWrite(string absolutePath, byte[] dataToWrite)
    {
        if (!IsPathAllowed(absolutePath))
            return false;
            
        var directory = Path.GetDirectoryName(absolutePath) ?? "";
        if (string.IsNullOrEmpty(directory))
            return false;
            
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
            
        await using var stream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.WriteAsync(dataToWrite, 0, dataToWrite.Length);
        
        return true;
    }

    public async Task<byte[]> ReadAsync(string absolutePath)
    {
        try
        {
            return await TryRead(absolutePath);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private async Task<byte[]> TryRead(string absolutePath)
    {
        if (!IsPathAllowed(absolutePath))
            return [];

        if (!Path.Exists(absolutePath))
            return [];
        
        await using var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var buffer = new byte[8192];
        var totalBytesRead = 0;
        var memoryStream = new MemoryStream();

        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            totalBytesRead += bytesRead;
            CheckFileSize(totalBytesRead);
            
            await memoryStream.WriteAsync(buffer, 0, bytesRead);
        }
        
        return memoryStream.ToArray();
    }

    private void CheckFileSize(long fileLength)
    {
        if (fileLength > _maxFileSize)
            throw new SecurityException(
                $"File size '{fileLength}' exceeds the allowable size '{_maxFileSize}'.");
    }
}