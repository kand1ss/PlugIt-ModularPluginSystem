using System.ComponentModel.DataAnnotations;
using ModularPluginAPI.Models;

namespace ModularPluginAPI.Components;

public static class MetadataValidator
{
    public static void Validate(AssemblyMetadata metadata)
    {
        var pluginsMetadata = metadata.Plugins;
        var duplicates = pluginsMetadata
            .GroupBy(item => item.Name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key).ToList();
        
        if(duplicates.Any())
            throw new ValidationException($"Assembly has duplicate plugin names: [{string.Join(',', duplicates)}].");
    }
    
    public static void Validate(IEnumerable<AssemblyMetadata> metadata)
        => metadata.ToList().ForEach(Validate);
}