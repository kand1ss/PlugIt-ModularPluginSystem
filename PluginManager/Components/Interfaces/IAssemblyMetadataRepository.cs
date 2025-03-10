using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyMetadataRepository
{
    void Add(AssemblyMetadata metadata);
    void AddRange(IEnumerable<AssemblyMetadata> metadata);
    void Remove(AssemblyMetadata metadata);
    void RemoveRange(IEnumerable<AssemblyMetadata> metadata);
    void Clear();
    
    AssemblyMetadata? GetMetadataByAssemblyName(string assemblyName);
    IEnumerable<AssemblyMetadata> GetAllMetadata();
    AssemblyMetadata? GetMetadataByPluginName(string pluginName);
}