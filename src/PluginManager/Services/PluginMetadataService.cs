using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class PluginMetadataService(IAssemblyMetadataRepository repository) : IPluginMetadataService
{
    public AssemblyMetadata GetMetadata(string assemblyPath)
        => repository.GetMetadataByAssemblyPath(assemblyPath)
               ?? throw new AssemblyNotFoundException(assemblyPath);

    public AssemblyMetadata GetMetadataByPluginName(string pluginName)
        => repository.GetMetadataByPluginName(pluginName)
           ?? throw new PluginNotFoundException(pluginName);
    public IEnumerable<PluginMetadata> GetAllPluginsMetadata()
        => repository.GetAllMetadata()
            .SelectMany(m => m.Plugins)
            .ToHashSet();

    public IEnumerable<string> GetPluginNamesFromMetadata(AssemblyMetadata metadata)
        => metadata.Plugins.Select(x => x.Name);

    public IEnumerable<string> GetAllAssembliesPaths()
        => repository.GetAllMetadata().Select(m => m.Path);

    public PluginMetadata GetPluginMetadataFromAssembly(AssemblyMetadata metadata, string pluginName)
        => metadata.Plugins.FirstOrDefault(p => p.Name == pluginName)
           ?? throw new PluginNotFoundException(pluginName);
}