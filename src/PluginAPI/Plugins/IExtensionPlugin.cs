namespace PluginAPI;

public interface IExtensionPlugin<T> : IPluginData
{
    void Expand(ref T data);
}