using System.Security;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;

namespace ModularPluginAPI.Components;

public class SecurityService(IAssemblySecurityService assemblySecurity, IPluginPermissionSecurityService permissionSecurity, IAssemblyLoader loader, 
    IAssemblyHandler handler, PluginLoggingFacade logger) 
    : ISecurityService, IMetadataRepositoryObserver
{
    public SecuritySettings Settings { get; } = new();

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
        => assemblySecurity.AddBlockedNamespace(namespaceName);

    public bool RemoveBlockedNamespace(string namespaceName)
        => assemblySecurity.RemoveBlockedNamespace(namespaceName);
    
    public void AddFileSystemPermission(string fullPath, bool canRead = true, bool canWrite = true, bool recursive = true)
        => permissionSecurity.AddFileSystemPermission(fullPath, canRead, canWrite, recursive);

    public void AddNetworkPermission(string url, bool canRead = true, bool canWrite = true)
        => permissionSecurity.AddNetworkPermission(url, canRead, canWrite);
}