using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Logger.Exporters;

/// <summary>
/// A class responsible for exporting log messages to a file.
/// </summary>
/// <remarks>
/// If the file name is not provided, a default name is generated based on the current date and time.
/// If a file with the same name already exists, a numeric suffix (e.g., "(1)", "(2)", etc.) is added.
/// </remarks>
/// <param name="path">The directory where the log file will be saved.</param>
/// <param name="fileName">Optional. The name of the log file. If null, a default name is used.</param>
public class FileLogExporter(string path, string? fileName = null) : ILogExporter
{
    private string? _fileName = fileName;

    /// <summary>
    /// Exports log messages to a file.
    /// </summary>
    /// <param name="messages">A collection of log messages to be written to the file.</param>
    /// <remarks>
    /// If the file name is not specified, it is automatically generated in the format "Log_yyyy-MM-dd_HH-mm-ss.log".
    /// The method ensures that the file has a ".log" extension.
    /// If a file with the same name already exists, a numeric suffix (e.g., "(1)", "(2)", etc.) is added to create a unique file name.
    /// 
    /// <para><b>Note:</b> If there are 0 messages, the file will not be created.</para>
    /// </remarks>
    public void Export(IEnumerable<string> messages)
    {
        messages = messages.ToList();
        if (!messages.Any())
            return;
        
        if (_fileName is null)
        {
            var date = DateTime.Now;
            _fileName = $"Log_{date:yyyy-MM-dd_HH-mm-ss}.log";
        }
        if (!Path.HasExtension(_fileName))
            _fileName += ".log";
        
        var fullPath = Path.Combine(path, _fileName);
        int counter = 1;
        
        while (File.Exists(fullPath))
        {
            var fileExtension = Path.GetExtension(fullPath);
            var newFileName = Path.GetFileNameWithoutExtension(_fileName);
            newFileName += $" ({counter})" + fileExtension;
            
            fullPath = Path.Combine(path, newFileName);
            counter++;
        }
        
        File.AppendAllLines(fullPath, messages);
    }
}