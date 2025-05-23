﻿using ConsoleTest;
using ModularPluginAPI;
using ModularPluginAPI.Components.Logger.Exporters;

class Program
{
    static async Task Main(string[] args)
    {
        var manager = new PluginManager();
        var observerComponent = new Observer();
        manager.Tracker.AddObserver(observerComponent);
        
        manager.Security.ImportJsonConfiguration(@"C:\Users\kand1s\Desktop\C# Projects\ModularPluginSystem\tests\ConsoleTest\SecurityConfig.json");
        manager.RegisterAssembliesFromDirectory(@"C:\Users\kand1s\Desktop\Plugins");

        manager.ExecutePlugin("ConsolePlugin");
        manager.ExecutePlugin("ConsolePlugin2");
        Console.ReadLine();
        await manager.ExecuteNetworkPluginAsync("NetworkPlugin", true, [100, 55, 24, 42]);
        manager.ExecutePlugin("FaultedPlugin");
        Console.ReadLine();
        manager.Tracker.RemoveObserver(observerComponent);
        manager.ExportDebugLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs"));
        manager.ExportTraceLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs"));
        manager.ExportProfilerLogs(new FileLogExporter(@"C:\Users\kand1s\Desktop\Logs", "ProfilerLog_1"));
        
        foreach(var info in manager.Tracker.GetPluginsStatus())
            Console.WriteLine($"{info.Name} : {info.Author} - {info.Version} = {info.CurrentState}");
        
        Console.WriteLine();
        
        foreach (var error in manager.ErrorRegistry.GetErrors("FaultedPlugin"))
            Console.WriteLine($"{error.PluginName} - {error.ExceptionType} {error.ErrorMessage} - {error.Timestamp} - {error.StateBeforeError}");
        manager.ExecutePlugin("FaultedPlugin");
        Console.ReadLine();
        foreach (var error in manager.ErrorRegistry.GetErrors("FaultedPlugin"))
            Console.WriteLine($"{error.PluginName} - {error.ExceptionType} {error.ErrorMessage} - {error.Timestamp} - {error.StateBeforeError}");
        
        manager.ErrorRegistry.RemoveErrorsByException("FaultedPlugin", typeof(Exception));
        Console.ReadLine();
        foreach (var error in manager.ErrorRegistry.GetErrors("FaultedPlugin"))
            Console.WriteLine($"{error.PluginName} - {error.ExceptionType} {error.ErrorMessage} - {error.Timestamp} - {error.StateBeforeError}");
    }
}