namespace PluginAPI;

public interface IInitialisablePlugin : IPlugin
{
    Task Initialize();
}