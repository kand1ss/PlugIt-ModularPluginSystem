using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public interface IAssemblyMetadataRepository
{
    void Add(AssemblyMetadata metadata);
    void AddRange(IEnumerable<AssemblyMetadata> metadata);
    void Remove(string assemblyPath);
    void Clear();
    
    AssemblyMetadata? GetMetadataByAssemblyPath(string assemblyPath);
    AssemblyMetadata? GetMetadataByPluginName(string pluginName);
    IEnumerable<AssemblyMetadata> GetAllMetadata();
}