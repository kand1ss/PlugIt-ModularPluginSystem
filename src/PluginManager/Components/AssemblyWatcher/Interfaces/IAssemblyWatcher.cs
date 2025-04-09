using ModularPluginAPI.Components.AssemblyWatcher.Observer;

namespace ModularPluginAPI.Components.AssemblyWatcher.Interfaces;

public interface IAssemblyWatcher : IObservableAssemblyWatcher
{
    void ObserveAssembly(string assemblyPath);
    void UnobserveAssembly(string assemblyPath);
}