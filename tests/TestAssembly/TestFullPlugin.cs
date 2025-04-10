using PluginAPI;

namespace PluginManagerTests.Test_Plugins;

public class TestFullPlugin : IInitialisablePlugin, IExecutablePlugin, IFinalisablePlugin
{
    public string Name { get; } = string.Empty;
    public Version Version { get; } = new(1,0,0);
    public string Description { get; } = string.Empty;
    public string Author { get; } = string.Empty;
    
    
    public bool IsInitialized { get; private set; } = false;
    public bool IsExecuted { get; private set; } = false;
    public bool IsFinalized { get; private set; } = false;


    public void Initialize()
        => IsInitialized = true;

    public void Execute()
        => IsExecuted = true;

    public void FinalizePlugin()
        => IsFinalized = true;
}