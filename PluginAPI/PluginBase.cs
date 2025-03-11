
namespace PluginAPI;

public abstract class PluginBase : IInitialisablePlugin, IExecutablePlugin, IFinalisablePlugin
{
    public abstract string Name { get; }
    public abstract Version Version { get; }
    public abstract string Description { get; }
    public abstract string Author { get; }

    public abstract void Initialize();
    public abstract void Execute();
    public abstract void FinalizePlugin();
}