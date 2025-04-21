namespace PluginAPI.Services;

/// <summary>
/// Contains configuration settings for the file system service, such as maximum file size.
/// </summary>
public class FileSystemServiceSettings
{
    private long _maxFileSizeBytes = 50 * 1024 * 1024;

    /// <summary>
    /// Gets or initializes the maximum file size allowed, in megabytes (MB).
    /// </summary>
    /// <remarks>
    /// This property provides a value representing the maximum file size allowed for file operations within the service.
    /// The value is both gettable and settable, and when set, it internally adjusts the value to bytes for internal storage.
    /// </remarks>
    public long MaxFileSizeMb
    {
        get => _maxFileSizeBytes / (1024 * 1024);
        set => _maxFileSizeBytes = value * 1024 * 1024;
    }

    /// <summary>
    /// Specifies the maximum allowed file size in bytes for the file system service.
    /// This property represents the configured limit for file size and acts as
    /// a computed value based on <see cref="MaxFileSizeMb"/>.
    /// </summary>
    public long MaxFileSizeBytes => _maxFileSizeBytes;
}