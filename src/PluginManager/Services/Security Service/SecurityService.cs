using System.Security;
using System.Text.Json;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using PluginAPI.Models.Permissions;
using PluginInfrastructure;

namespace ModularPluginAPI.Components;

public class SecurityService(SecuritySettingsProvider settingsProvider, IAssemblySecurityService assemblySecurity, 
    IPluginPermissionSecurityService permissionSecurity, IAssemblyLoader loader, IAssemblyHandler handler, 
    PluginLoggingFacade logger) 
    : ISecurityService, IMetadataRepositoryObserver
{
    private SecuritySettings _settings => settingsProvider.Settings;
    public ISecuritySettings Settings => _settings;


    private readonly HashSet<string> _unrecommendedNamespaces =
    [
        "System.IO",
        "System.Reflection.Emit",
        "System.Reflection",
        "System.Linq.Expressions",
        "System.Net"
    ];

    public bool ImportJsonConfiguration(string configurationPath)
    {
        if (!configurationPath.EndsWith(".json"))
            return false;
        
        var json = File.ReadAllText(configurationPath);
        var settings = TryReadJson(json);
        if (settings is null)
            return false;
        
        settingsProvider.UpdateSettings(settings);
        logger.SecuritySettingsImported(configurationPath);
        CheckSettingsNamespaces();
        return true;
    }


    private void CheckSettingsNamespaces()
    {
        var missingNamespaces = _unrecommendedNamespaces.Except(_settings.ProhibitedNamespaces).ToList();
        if (missingNamespaces.Count == 0)
            return;
        
        foreach (var namespaceName in missingNamespaces)
            logger.SecuritySettingsNotHasUnrecommendedNamespace(namespaceName);
    }
    

    private SecuritySettings? TryReadJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<SecuritySettings>(json);
        }
        catch
        {
            return null;
        }
    }
    

    
    public void OnMetadataAdded(AssemblyMetadata assemblyMetadata)
    {
        if (!CheckSafety(assemblyMetadata))
            throw new SecurityException(
                $"Assembly '{assemblyMetadata.Name} v{assemblyMetadata.Version}' does not meet the specified safety standards.");
    }
    public void OnMetadataRemoved(AssemblyMetadata assemblyMetadata)
    {
    }
    
    private bool CheckSafety(AssemblyMetadata assemblyMetadata)
    {
        var result = assemblySecurity.CheckSafety(assemblyMetadata.Path) &&
               assemblyMetadata.Plugins.All(permissionSecurity.CheckSafety);

        if (result)
        {
            logger.SecurityCheckPassed(assemblyMetadata.Name, assemblyMetadata.Version);
            return true;
        }
        
        logger.SecurityCheckFailed(assemblyMetadata.Name, assemblyMetadata.Version);
        return false;
    }

    public bool CheckAssemblySafety(string assemblyPath)
    {
        var metadata = CreateMetadata(assemblyPath);
        var result = CheckSafety(metadata);
        
        loader.UnloadAssembly(assemblyPath);
        return result;
    }

    private AssemblyMetadata CreateMetadata(string assemblyPath)
    {
        var metadataGenerator = new AssemblyMetadataGenerator(handler);
        var assembly = loader.LoadAssembly(assemblyPath);

        return metadataGenerator.Generate(assembly);
    }
    public bool AddBlockedNamespace(string namespaceName)
        => _settings.AddBlockedNamespace(namespaceName);
    
    public bool RemoveBlockedNamespace(string namespaceName)
        => _settings.RemoveBlockedNamespace(namespaceName);
    
    
    public void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = true)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(fullPath);
        _settings.AddFileSystemPermission(new FileSystemPermission(normalizedPath, canRead, canWrite, recursive));
        
        logger.FilePermissionAdded(fullPath, canRead, canWrite);
    }
    public void AddNetworkPermission(string url, bool canGet = true, bool canPost = true)
    {
        var normalizedUrl = Normalizer.NormalizeUrl(url);
        _settings.AddNetworkPermission(new NetworkPermission(normalizedUrl, canGet, canPost));
        
        logger.NetworkPermissionAdded(url, canGet, canPost);
    }
}