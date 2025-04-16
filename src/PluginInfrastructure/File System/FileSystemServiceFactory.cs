using PluginAPI.Services.interfaces;

namespace PluginAPI.Services;

public class FileSystemServiceFactory
{
    public static IPluginFileSystemService Create(IFileSystemPermissionController controller)
        => new PluginFileSystemService(controller);
}