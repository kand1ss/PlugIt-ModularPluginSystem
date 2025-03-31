using ModularPluginAPI.Components.Interfaces.Services;
using ModularPluginAPI.Components.Logger;

namespace ModularPluginAPI.Components;

public class PluginMetadataDispatcher(IAssemblyMetadataRepository repository, IPluginMetadataService metadataService, 
    IAssemblyLoader loader, IAssemblyHandler handler, IPluginTracker tracker, 
    PluginLoggingFacade logger)
{
    private readonly AssemblyMetadataGenerator _metadataGenerator = new(handler);

    public void RemoveMetadata(string assemblyPath)
    {
        var metadata = metadataService.GetMetadata(assemblyPath);
        var plugins = metadataService.GetPluginNamesFromMetadata(metadata);
        
        repository.Remove(assemblyPath);
        tracker.RemovePlugins(plugins);
        logger.MetadataRemoved(assemblyPath, metadata.Version);
    }

    public void RemoveMetadataFromDirectory(string directoryPath)
    {
        var assemblies = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach(var assembly in assemblies)
            RemoveMetadata(assembly);
    }
    
    public void LoadMetadata(string assemblyPath)
    {
        var assembly = loader.LoadAssembly(assemblyPath);
        var metadata = _metadataGenerator.Generate(assembly);

        MetadataValidator.Validate(metadata);
        repository.Add(metadata);
        
        logger.MetadataAdded(assemblyPath, metadata.Version);
        tracker.RegisterPlugins(metadata.Plugins);
    }

    public void LoadMetadataFromDirectory(string directoryPath)
    {
        var assemblies = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach(var assembly in assemblies)
            LoadMetadata(assembly);
    }
}
