using System.Security;
using PluginAPI.Models.Permissions;
using PluginAPI.Models.Permissions.Interfaces;
using PluginAPI.Services.interfaces;

namespace PluginAPI.Services;

public class PluginFileSystemService : IPluginFileSystemService
{
    private readonly Dictionary<string, FileSystemPermission> _allowedDirectories = new();
    private readonly long _maxFileSize;

    private readonly FileSystemPermissionChecker _permissionChecker;

    public PluginFileSystemService(IPermissionController<FileSystemPermission> controller, FileSystemServiceSettings? settings = null)
    {
        settings ??= new();
        foreach (var permission in controller.GetPermissions())
            _allowedDirectories.Add(permission.Path, permission);
        
        _maxFileSize = settings.MaxFileSizeBytes;
        _permissionChecker = new(_allowedDirectories);
    }

    private void IsPathAllowed(string path, bool isRead)
    {
        if(!_permissionChecker.CheckPermissionAllow(path, out var permission) || permission == null)
            throw new SecurityException($"Path '{path}' is not permitted.");

        CheckAccess(path, permission, isRead);
    }

    private void CheckAccess(string path, FileSystemPermission permission, bool isRead)
    {
        if (isRead && !permission.CanRead)
            throw new SecurityException($"Path '{path}' does not allow read.");
        if (!isRead && !permission.CanWrite)
            throw new SecurityException($"Path '{path}' does not allow write.");
    }

    public async Task WriteAsync(string absolutePath, byte[] dataToWrite)
    {
        IsPathAllowed(absolutePath, false);
            
        var directory = Path.GetDirectoryName(absolutePath) ?? "";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        CheckFileSize(dataToWrite.Length);
            
        await using var stream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.WriteAsync(dataToWrite, 0, dataToWrite.Length);
    }

    public async Task<byte[]> ReadAsync(string absolutePath)
    {
        IsPathAllowed(absolutePath, true);
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