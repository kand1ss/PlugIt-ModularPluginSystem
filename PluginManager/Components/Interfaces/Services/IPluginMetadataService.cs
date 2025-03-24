using ModularPluginAPI.Exceptions;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components.Interfaces.Services;

public interface IPluginMetadataService
{
    AssemblyMetadata GetMetadata(string assemblyName);
    AssemblyMetadata GetMetadataByPluginName(string pluginName);
    IEnumerable<string> GetPluginNamesFromMetadata(AssemblyMetadata assemblyName);
    PluginMetadata GetPluginMetadataFromAssembly(AssemblyMetadata metadata, string pluginName);
}