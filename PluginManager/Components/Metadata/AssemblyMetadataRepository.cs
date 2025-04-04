using System.Data;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyMetadataRepository : IAssemblyMetadataRepository
{
    private readonly List<IMetadataRepositoryObserver> _observers = new();
    private readonly Dictionary<string, AssemblyMetadata> _assemblies = new();


    private void NotifyObservers(Action<IMetadataRepositoryObserver> action)
    {
        foreach(var observer in _observers)
            action(observer);
    }
    public void AddObserver(IMetadataRepositoryObserver observer)
        => _observers.Add(observer);
    public void RemoveObserver(IMetadataRepositoryObserver observer)
        => _observers.Remove(observer);

    private void CheckPluginsForDuplicate(AssemblyMetadata metadata)
    {
        var allExistingPluginNames = _assemblies.Values.SelectMany(a => a.Plugins);
        var allNewPluginNames = metadata.Plugins;

        var duplicatePluginNames = allNewPluginNames.Intersect(allExistingPluginNames).ToList();
        if (duplicatePluginNames.Count == 0)
            return;
            
        throw new DuplicateNameException(
                $"Assembly contains plugin names that are contained in another assembly: [{string.Join(',', duplicatePluginNames)}]");
    }
    
    public void Add(AssemblyMetadata metadata)
    {
        CheckPluginsForDuplicate(metadata);
        if(_assemblies.TryAdd(metadata.Path, metadata))
            NotifyObservers(o => o.OnMetadataAdded(metadata));
            
    }
    public void AddRange(IEnumerable<AssemblyMetadata> metadata)
        => metadata.ToList().ForEach(Add);
    
    
    public void Remove(string assemblyName)
    {
        var metadata = _assemblies[assemblyName];
        if(_assemblies.Remove(assemblyName))
            NotifyObservers(o => o.OnMetadataRemoved(metadata));
    }

    public void Clear()
    {
        _assemblies.Clear();
        NotifyObservers(o => o.OnMetadataCleared());
    }


    public AssemblyMetadata? GetMetadataByAssemblyPath(string assemblyPath)
        => _assemblies.GetValueOrDefault(assemblyPath);
    public AssemblyMetadata? GetMetadataByPluginName(string pluginName)
        => _assemblies.FirstOrDefault(x => x.Value.Plugins
            .Select(m => m.Name)
            .Contains(pluginName)).Value;
    public IEnumerable<AssemblyMetadata> GetAllMetadata()
        => _assemblies.Values;
}