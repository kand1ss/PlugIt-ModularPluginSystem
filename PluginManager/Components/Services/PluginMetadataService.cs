using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class PluginMetadataService(IAssemblyMetadataRepository repository) : IPluginMetadataService
{
    public AssemblyMetadata GetMetadata(string assemblyName)
        => repository.GetMetadataByAssemblyName(assemblyName)
           ?? throw new AssemblyNotFoundException(assemblyName);
    public AssemblyMetadata GetMetadataByPluginName(string pluginName)
        => repository.GetMetadataByPluginName(pluginName)
           ?? throw new PluginNotFoundException(pluginName);
    
    public IEnumerable<string> GetPluginNamesFromMetadata(AssemblyMetadata metadata)
        => metadata.Plugins.Select(x => x.Name);
    public PluginMetadata GetPluginMetadataFromAssembly(AssemblyMetadata metadata, string pluginName)
        => metadata.Plugins.FirstOrDefault(p => p.Name == pluginName)
           ?? throw new PluginNotFoundException(pluginName);
}