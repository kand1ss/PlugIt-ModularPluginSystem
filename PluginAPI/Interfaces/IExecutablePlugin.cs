namespace PluginAPI;

public interface IExecutablePlugin : IPlugin
{
    Task ExecuteAsync();
}