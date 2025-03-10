using System.Reflection;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public class AssemblyMetadataGenerator(IAssemblyHandler assemblyHandler)
{
    public AssemblyMetadata Generate(Assembly assembly)
    {
        var pluginsInAssembly = assemblyHandler.GetAllPlugins(assembly);
        
        var metadata = new AssemblyMetadata
        {
            Name = assembly.GetName().Name ?? "null",
            Path = assembly.Location,
            Version = assembly.GetName().Version ?? new (0, 0, 0),
            Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "null",
            Plugins = PluginMetadataGenerator.Generate(pluginsInAssembly).ToHashSet(),
        };
        
        return metadata;
    }
    
    public IEnumerable<AssemblyMetadata> Generate(IEnumerable<Assembly> assemblies)
        => assemblies.Select(Generate);
}