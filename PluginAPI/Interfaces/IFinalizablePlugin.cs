namespace PluginAPI;

public interface IFinalisablePlugin : IPlugin
{
    Task FinalizeAsync();
}