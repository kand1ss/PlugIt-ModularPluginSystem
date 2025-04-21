using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using ModularPluginAPI.Services.Interfaces;
using Mono.Cecil;
using PluginAPI.Services.interfaces;

namespace ModularPluginAPI.Components;

public class AssemblySecurityService : IAssemblySecurityService
{
    private readonly HashSet<string> _blockedNamespaces = new()
    {
        "System.IO",
        "System.Reflection.Emit",
        "System.Reflection",
        "System.Linq.Expressions",
        "System.Net"
    };

    private readonly HashSet<string> _trustedTokens = new()
    {
        "b03f5f7f11d50a3a",
        "7cec85d7bea7798e"
    };
    
    private readonly HashSet<string> _checkedAssemblies = new();
    
    
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
        if (!_checkedAssemblies.Add(assemblyPath))
            return true;

        var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
        var resolver = new AssemblyDependencyResolver(assemblyPath);
        bool result = AnalyzeNamespaces(assembly) && AnalyzeInterfaces(assembly);
        
        foreach (var reference in assembly.MainModule.AssemblyReferences)
        {
            var resolvedPath = resolver.ResolveAssemblyToPath(new AssemblyName(reference.FullName));
            if (IsTrustedAssembly(reference, resolvedPath ?? ""))
                continue;
            
            if (resolvedPath != null)
                result &= CheckSafety(resolvedPath);
        }

        return result;
    }

    private bool IsTrustedAssembly(AssemblyNameReference reference, string resolvedPath)
    {
        var name = reference.Name;
        var publicKey = BitConverter.ToString(reference.PublicKeyToken).Replace("-", "").ToLowerInvariant();

        return name == "PluginAPI" 
                     || _trustedTokens.Contains(publicKey ?? "")
                     || resolvedPath.StartsWith(RuntimeEnvironment.GetRuntimeDirectory(), StringComparison.OrdinalIgnoreCase);
    }


    private bool AnalyzeNamespaces(AssemblyDefinition assembly)
    {
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

    private bool AnalyzeInterfaces(AssemblyDefinition assembly)
    {
        foreach (var type in assembly.MainModule.Types)
        {
            foreach (var @interface in type.Interfaces)
            {
                if (@interface.InterfaceType.FullName == typeof(IPluginFileSystemService).FullName)
                    return false;
                
                if (@interface.InterfaceType.FullName == typeof(IPluginNetworkService).FullName)
                    return false;
            }
        }

        return true;
    }
}