namespace PluginAPI;

public interface IConfigurablePlugin : IPluginData
{
    PluginConfiguration? Configuration { get; }
    void LoadConfiguration(PluginConfiguration? configuration);
}