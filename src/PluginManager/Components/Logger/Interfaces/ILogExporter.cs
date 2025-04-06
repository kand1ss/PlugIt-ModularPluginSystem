namespace ModularPluginAPI.Components.Logger.Interfaces;

public interface ILogExporter
{
    void Export(IEnumerable<string> messages);
}