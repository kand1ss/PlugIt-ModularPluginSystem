using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ModularPluginAPI.Components.JsonConverters;
using PluginAPI.Models.Permissions;
using PluginAPI.Services;
using PluginInfrastructure;
using PluginInfrastructure.Network_Service;

namespace ModularPluginAPI.Components;

public class SecuritySettings : ISecuritySettings
{
    public IReadOnlyCollection<string> ProhibitedNamespaces => BlockedNamespaces;
    
    [JsonInclude] 
    private HashSet<string> BlockedNamespaces { get; init; } =
    [
        "System.IO",
        "System.Reflection.Emit",
        "System.Reflection",
        "System.Linq.Expressions",
        "System.Net"
    ];


    [JsonConverter(typeof(PermissionDictionaryConverter<FileSystemPermission>))]
    [JsonInclude]
    public Dictionary<string, FileSystemPermission> FileSystemPermissions { get; private set; } = new();


    [JsonConverter(typeof(PermissionDictionaryConverter<NetworkPermission>))]
    [JsonInclude]
    public Dictionary<string, NetworkPermission> NetworkPermissions { get; private set; } = new();


    public NetworkServiceSettings Network { get; init; } = new();
    public FileSystemServiceSettings FileSystem { get; init; } = new();


    public bool AddBlockedNamespace(string namespaceName)
    {
        if (!IsNamespaceValid(namespaceName))
            return false;
        
        return BlockedNamespaces.Add(namespaceName);
    }

    private bool IsNamespaceValid(string namespaceName)
    {
        var regularExpression = new Regex(@"^([A-Za-z]+)(\.([A-Za-z]+))*$", RegexOptions.Compiled);
        return regularExpression.IsMatch(namespaceName);
    }
    
    public bool RemoveBlockedNamespace(string namespaceName)
        => BlockedNamespaces.Remove(namespaceName);
    
    public void AddFileSystemPermission(FileSystemPermission permission)
    {
        var normalizedPath = Normalizer.NormalizeDirectoryPath(permission.Path);
        FileSystemPermissions[normalizedPath] = permission;
    }

    public void AddNetworkPermission(NetworkPermission permission)
    {
        var normalizedPath = Normalizer.NormalizeUrl(permission.Path);
        NetworkPermissions[normalizedPath] = permission;
    }
}