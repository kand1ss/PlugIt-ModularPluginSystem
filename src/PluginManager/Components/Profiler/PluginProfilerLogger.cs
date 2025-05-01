using ModularPluginAPI.Components.Logger.Interfaces;

namespace ModularPluginAPI.Components.Profiler;

public class PluginProfilerLogger
{
    private readonly List<string> _dataToLog = new();
    
    public void CreateLog(ProfiledData profiledData)
        => _dataToLog.Add(ConstructMessage(profiledData));

    public void Export(ILogExporter exporter)
    {
        exporter.Export(_dataToLog);
        _dataToLog.Clear();
    }

    private string ConstructMessage(ProfiledData data)
    {
        var message = $"({data.Created}) [{data.PluginName}]";
        
        if(data.ItWasExecuted)
            message += $" Running time: {data.ExecutingTimeMs}ms |";

        return message.TrimEnd('|');
    }
}