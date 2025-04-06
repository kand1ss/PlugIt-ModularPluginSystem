namespace PluginAPI;

public interface IConfigurablePlugin : IPlugin
{
    PluginConfiguration? Configuration { get; }
    void LoadConfiguration(PluginConfiguration? configuration);
}