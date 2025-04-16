namespace PluginAPI.Services.interfaces;

public interface IPluginFileSystemService
{
    bool Write(string absolutePath, byte[] dataToWrite);
    byte[] Read(string absolutePath);
}