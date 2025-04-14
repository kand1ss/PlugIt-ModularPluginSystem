using System.Security;
using System.Text.RegularExpressions;
using ModularPluginAPI.Components.Observer;
using ModularPluginAPI.Models;
using ModularPluginAPI.Services.Interfaces;
using Mono.Cecil;

namespace ModularPluginAPI.Components;

public class AssemblySecurityService : IAssemblySecurityService, IMetadataRepositoryObserver
{
    private readonly HashSet<string> _blockedNamespaces = new()
    {
        "System.IO",
        "System.Reflection.Emit",
        "System.Net"
    };
    
    public void OnMetadataAdded(AssemblyMetadata assemblyMetadata)
    {
        if (!CheckSafety(assemblyMetadata.Path))
            throw new SecurityException(
                $"Assembly '{assemblyMetadata.Name} v{assemblyMetadata.Version}' does not meet the specified safety standards.");
    }

    public void OnMetadataRemoved(AssemblyMetadata assemblyMetadata)
    {
    }

    public void OnMetadataCleared()
    {
    }
    

    public bool AddBlockedNamespace(string namespaceName)
    {
        var regularExpression = new Regex(@"^([A-Za-z]+)(\.([A-Za-z]+))*$", RegexOptions.Compiled);
        if (!regularExpression.IsMatch(namespaceName))
            return false;
        
        return _blockedNamespaces.Add(namespaceName);
    }

    public bool RemoveBlockedNamespace(string namespaceName)
        => _blockedNamespaces.Remove(namespaceName);

    public bool CheckSafety(string assemblyPath)
    {
        var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
        
        foreach (var module in assembly.Modules)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is MethodReference methodRef)
                        {
                            var typeNamespace = methodRef.DeclaringType.Namespace;
                            if (_blockedNamespaces.Any(banned => typeNamespace.StartsWith(banned)))
                                return false;
                        }
                    }
                }
            }
        }

        return true;
    }
}