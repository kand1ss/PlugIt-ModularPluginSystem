using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyMetadataRepository
{
    void Add(AssemblyMetadata metadata);
    void AddRange(IEnumerable<AssemblyMetadata> metadata);
    void Remove(string assemblyName);
    void Clear();
    
    AssemblyMetadata? GetMetadataByAssemblyName(string assemblyName);
    AssemblyMetadata? GetMetadataByPluginName(string pluginName);
}