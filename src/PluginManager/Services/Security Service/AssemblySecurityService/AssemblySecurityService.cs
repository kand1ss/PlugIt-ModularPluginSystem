using System.Reflection;
using System.Runtime.Loader;
using ModularPluginAPI.Services.Interfaces;
using Mono.Cecil;

namespace ModularPluginAPI.Components;

public class AssemblySecurityService(IAssemblyAnalyzer analyzer) : IAssemblySecurityService
{
    private readonly HashSet<string> _trustedTokens =
    [
        "b03f5f7f11d50a3a",
        "7cec85d7bea7798e",
        "2c2e8d52f28b9f5e"
    ];
    
    private readonly HashSet<string> _checkedAssemblies = new();

    
    public bool CheckSafety(string assemblyPath)
    {
        if (!_checkedAssemblies.Add(assemblyPath))
            return true;

        var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
        var resolver = new AssemblyDependencyResolver(assemblyPath);

        return analyzer.Analyze(assembly) && ResolveAndCheckAssemblyReferences(assembly, resolver);
    }

    private bool ResolveAndCheckAssemblyReferences(AssemblyDefinition assembly, AssemblyDependencyResolver resolver)
    {
        var result = true;
        foreach (var reference in assembly.MainModule.AssemblyReferences)
        {
            var resolvedPath = resolver.ResolveAssemblyToPath(new AssemblyName(reference.FullName));
            if (IsTrustedAssembly(reference))
                continue;
            
            if (resolvedPath != null)
                result &= CheckSafety(resolvedPath);
        }

        return result;
    }

    private bool IsTrustedAssembly(AssemblyNameReference reference)
    {
        var publicKey = BitConverter.ToString(reference.PublicKeyToken).Replace("-", "").ToLowerInvariant();
        return _trustedTokens.Contains(publicKey);
    }
}