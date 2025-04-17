using System.Reflection;
using System.Runtime.Loader;
using System.Security;
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
        "System.Net"
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
            if (IsTrustedAssembly(reference.Name))
                continue;

            var resolvedPath = resolver.ResolveAssemblyToPath(new AssemblyName(reference.FullName));
            if (resolvedPath != null)
                result &= CheckSafety(resolvedPath);
        }

        return result;
    }
    
    private bool CheckSafety(AssemblyDefinition assembly)
        => AnalyzeNamespaces(assembly) && AnalyzeInterfaces(assembly);

    private bool IsTrustedAssembly(string name)
    {
        return name.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
               name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("PluginAPI", StringComparison.OrdinalIgnoreCase) ||
               name == "System" || name == "mscorlib" || name == "netstandard";
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
                    throw new SecurityException($"A fake secure file system service has been detected in the assembly: '{assembly.Name}'");
                if (@interface.InterfaceType.FullName == typeof(IPluginNetworkService).FullName)
                    throw new SecurityException($"A fake secure network service has been detected in the assembly: '{assembly.Name}'");
            }
        }

        return true;
    }
}