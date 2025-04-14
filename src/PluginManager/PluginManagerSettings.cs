namespace ModularPluginAPI;

/// <summary>
/// Represents the settings for the plugin manager, including options for profiling and error registry functionality.
/// </summary>
public class PluginManagerSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether profiling is enabled.
    /// If <c>true</c>, the plugin manager will record the execution time of various stages of the plugin's lifecycle 
    /// (initialization, execution, and finalization).
    /// If <c>false</c>, no execution time will be recorded.
    /// </summary>
    public bool EnableProfiling { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the error registry is enabled.
    /// If <c>true</c>, the plugin manager will log errors to an error registry whenever they occur during plugin execution.
    /// If <c>false</c>, errors will not be recorded in the error registry.
    /// </summary>
    public bool EnableErrorRegistry { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether the security service is enabled.
    /// If <c>true</c>, the plugin manager will enable security checks for plugins, including static analysis and safe service access.
    /// If <c>false</c>, security checks will be disabled.
    /// </summary>
    public bool EnableSecurity { get; set; } = true;
}
