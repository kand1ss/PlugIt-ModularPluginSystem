using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Interfaces.Services;

public interface IPluginMetadataService
{
    AssemblyMetadata GetMetadata(string assemblyName);
    AssemblyMetadata GetMetadataByPluginName(string pluginName);
    IEnumerable<string> GetPluginNamesFromMetadata(AssemblyMetadata metadata);
    IEnumerable<PluginMetadata> GetAllPluginsMetadata();
    PluginMetadata GetPluginMetadataFromAssembly(AssemblyMetadata metadata, string pluginName);
}