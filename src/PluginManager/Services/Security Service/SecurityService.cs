using System.Security;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;

namespace ModularPluginAPI.Components;

public class SecurityService(PluginLoggingFacade logger) : ISecurityService, IMetadataRepositoryObserver
{
    private readonly AssemblySecurityService _assemblySecurity = new();
    private readonly PluginPermissionSecurityService _pluginSecurity = new();
    

    public void OnMetadataAdded(AssemblyMetadata assemblyMetadata)
    {
        if (!CheckSafety(assemblyMetadata))
            throw new SecurityException(
                $"Assembly '{assemblyMetadata.Name} v{assemblyMetadata.Version}' does not meet the specified safety standards.");
    }
    public void OnMetadataRemoved(AssemblyMetadata assemblyMetadata)
    {
    }
    
    public bool CheckSafety(AssemblyMetadata assemblyMetadata)
    {
        var result = _assemblySecurity.CheckSafety(assemblyMetadata.Path) &&
               assemblyMetadata.Plugins.All(_pluginSecurity.CheckSafety);

        if (result)
        {
            logger.SecurityCheckPassed(assemblyMetadata.Name, assemblyMetadata.Version);
            return true;
        }
        
        logger.SecurityCheckFailed(assemblyMetadata.Name, assemblyMetadata.Version);
        return false;
    }

    public bool CheckAssemblySafety(string assemblyPath)
        => _assemblySecurity.CheckSafety(assemblyPath);

    public bool AddBlockedNamespace(string namespaceName)
        => _assemblySecurity.AddBlockedNamespace(namespaceName);

    public bool RemoveBlockedNamespace(string namespaceName)
        => _assemblySecurity.RemoveBlockedNamespace(namespaceName);
    
    public void AddFileSystemPermission(string fullPath)
        => _pluginSecurity.AddFileSystemPermission(fullPath);

    public void AddNetworkPermission(string url)
        => _pluginSecurity.AddNetworkPermission(url);
}