namespace ModularPluginAPI.Components;

public class SecuritySettingsProvider
{
    public SecuritySettings Settings { get; private set; } = new();
    
    public void UpdateSettings(SecuritySettings settings)
    {
        Settings = settings;
    }
}