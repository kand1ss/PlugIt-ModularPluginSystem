
namespace PluginAPI;

public interface IPluginData
{
    string Name { get; }
    Version Version { get; }
    string Description { get; }
    string Author { get; }
}