using PluginAPI.Services.interfaces;

namespace PluginAPI.Services;

internal class PluginFileSystemService : IPluginFileSystemService
{
    private readonly HashSet<string> _allowedDirectories = new();

    public PluginFileSystemService(IFileSystemPermissionController controller)
    {
        foreach (var directory in controller.GetAllowedDirectories())
            _allowedDirectories.Add(directory);
    }
    
    private bool IsPathInAllowedDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        
        if (string.IsNullOrEmpty(directory))
            return false;
            
        return _allowedDirectories.Contains(directory) &&
               !PathTraversesUp(path) &&
               !IsSymbolicLink(path);
    }
    
    private static bool PathTraversesUp(string path)
    {
        return path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(s => s == "..");
    }
    
    private static bool IsSymbolicLink(string path)
    {
        var fi = new FileInfo(path);
        return fi.Exists && (fi.Attributes & FileAttributes.ReparsePoint) != 0;
    }



    public bool Write(string absolutePath, byte[] dataToWrite)
    {
        try
        {
            return TryWrite(absolutePath, dataToWrite);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool TryWrite(string absolutePath, byte[] dataToWrite)
    {
        if (!IsPathInAllowedDirectory(absolutePath))
            return false;
            
        var directory = Path.GetDirectoryName(absolutePath) ?? "";
        if (string.IsNullOrEmpty(directory))
            return false;
            
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
            
        using var fs = new FileStream(absolutePath, FileMode.Create, FileAccess.Write, FileShare.None);
        fs.Write(dataToWrite, 0, dataToWrite.Length);
        return true;
    }

    public byte[] Read(string absolutePath)
    {
        try
        {
            return TryRead(absolutePath);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private byte[] TryRead(string absolutePath)
    {
        if (!IsPathInAllowedDirectory(absolutePath))
            return [];

        if (!Path.Exists(absolutePath))
            return [];

        return File.ReadAllBytes(absolutePath);
    }
}