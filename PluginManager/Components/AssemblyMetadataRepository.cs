using System.Data;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyMetadataRepository(ILoggerService logger) : IAssemblyMetadataRepository
{
    private readonly Dictionary<string, AssemblyMetadata> _assemblies = new();

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

        var assemblyName = metadata.Name;
        _assemblies.TryAdd(assemblyName, metadata);
    }
    public void AddRange(IEnumerable<AssemblyMetadata> metadata)
        => metadata.ToList().ForEach(Add);
    
    
    public void Remove(string assemblyName)
        => _assemblies.Remove(assemblyName);
    public void Clear() => _assemblies.Clear();


    public AssemblyMetadata? GetMetadataByAssemblyName(string assemblyName)
        => _assemblies.GetValueOrDefault(assemblyName);
    public AssemblyMetadata? GetMetadataByPluginName(string pluginName)
        => _assemblies.FirstOrDefault(x => x.Value.Plugins
            .Select(m => m.Name)
            .Contains(pluginName)).Value;
}