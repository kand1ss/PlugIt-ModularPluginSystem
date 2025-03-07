using ModularPluginAPI.Components;
using PluginAPI;

namespace ModularPluginAPI;

public class PluginManager
{
    private readonly PluginDispatcher _dispatcher;

    public PluginManager(string pluginsSource)
    {
        var pluginExtractor = new AssemblyExtractor(pluginsSource);
        var pluginRepository = new PluginRepository();
        var assemblyHandler = new AssemblyHandler();
        _dispatcher = new(pluginRepository, pluginExtractor, assemblyHandler);
    }
}