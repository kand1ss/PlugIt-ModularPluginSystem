using Microsoft.Extensions.Configuration;

namespace PluginAPI;

public interface IPlugin
{
    string Name { get; }
    Version Version { get; }
    string Description { get; }
    string Author { get; }
}