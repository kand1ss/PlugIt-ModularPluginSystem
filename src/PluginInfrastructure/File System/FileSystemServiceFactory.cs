using PluginAPI.Services.interfaces;

namespace PluginAPI.Services;

public class FileSystemServiceFactory
{
    public static IPluginFileSystemService Create(IFileSystemPermissionController controller, FileSystemServiceSettings? settings = null)
        => new PluginFileSystemService(controller, settings);
}