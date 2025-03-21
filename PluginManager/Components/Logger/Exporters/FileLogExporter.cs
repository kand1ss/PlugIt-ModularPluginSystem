using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Logger.Exporters;

public class FileLogExporter(string path) : ILogExporter
{
    public void Export(IEnumerable<string> messages)
    {
        var date = DateTime.Now;
        var fileName = $"Log_{date:yyyy-MM-dd_HH-mm-ss}.log";
        var fullPath = Path.Combine(path, fileName);
        
        File.AppendAllLines(fullPath, messages);
    }
}