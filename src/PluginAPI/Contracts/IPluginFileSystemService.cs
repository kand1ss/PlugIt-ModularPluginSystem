namespace PluginAPI.Services.interfaces;

/// <summary>
/// Provides methods to safety interact with the filesystem, specifically for reading and writing data.
/// </summary>
public interface IPluginFileSystemService
{
    /// <summary>
    /// Writes the specified data to a file at the given absolute path.
    /// Ensures that the path is within an allowed directory and creates the necessary directories if they do not exist.
    /// </summary>
    /// <param name="absolutePath">The complete path of the file where the data should be written.</param>
    /// <param name="dataToWrite">The binary data to write to the file.</param>
    /// <returns><c>true</c> if the data was successfully written to the file; otherwise, <c>false</c>.</returns>
    bool Write(string absolutePath, byte[] dataToWrite);


    /// <summary>
    /// Reads the content of a file located at the specified absolute path.
    /// Ensures that the path is within an allowed directory and the file exists.
    /// </summary>
    /// <param name="absolutePath">The complete path of the file to be read.</param>
    /// <returns>The binary content of the file if it exists and is accessible; otherwise, an empty array.</returns>
    byte[] Read(string absolutePath);
}