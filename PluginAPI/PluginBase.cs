namespace PluginAPI;

public abstract class PluginBase : IInitialisablePlugin, IExecutablePlugin, IFinalisablePlugin
{
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Description { get; }

    public abstract Task Initialize();
    public abstract Task ExecuteAsync();
    public abstract Task FinalizeAsync();
}