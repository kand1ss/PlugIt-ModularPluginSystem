namespace PluginAPI;

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
}