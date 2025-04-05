using ConsoleTest;
using ModularPluginAPI;
using ModularPluginAPI.Components.Logger.Exporters;

class Program
{
    static void Main(string[] args)
    {
        var manager = new PluginManager();
        manager.RegisterAssembliesFromDirectory(@"C:\Users\kand1s\Desktop\Plugins");
        var observerComponent = new Observer();
        manager.Tracker.AddObserver(observerComponent);
        manager.ExecutePlugin("ConsolePlugin");
        manager.ExecutePlugin("ConsolePlugin2");
        Console.ReadLine();
        manager.ExecuteNetworkPlugin("NetworkPlugin", true, [100, 55, 24, 42]);
        manager.ExecutePlugin("FaultedPlugin");
        Console.ReadLine();
        manager.Tracker.RemoveObserver(observerComponent);
        manager.ExportDebugLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs"));
        manager.ExportTraceLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs"));
        manager.ExportProfilerLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs", "ProfilerLog_1"));

        foreach(var info in manager.Tracker.GetPluginsStatus())
            Console.WriteLine($"{info.Name} : {info.Author} - {info.Version} = {info.State}");
    }
}