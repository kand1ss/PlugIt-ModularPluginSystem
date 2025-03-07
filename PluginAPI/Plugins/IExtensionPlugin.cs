namespace PluginAPI;

public interface IExtensionPlugin<T> : IPlugin
{
    void Expand(ref T data);
}