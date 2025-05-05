using ModularPluginAPI.Services.Interfaces;
using Mono.Cecil;
using PluginAPI.Services.interfaces;

namespace ModularPluginAPI.Components.Modules;

public class AssemblyAnalyzer(SecuritySettingsProvider settingsProvider) : IAssemblyAnalyzer
{
    private readonly HashSet<string> _prohibitedInterfacesNames =
    [
        typeof(IPluginFileSystemService).FullName ?? "",
        typeof(IPluginNetworkService).FullName ?? ""
    ];
    
    
    public bool Analyze(AssemblyDefinition assembly)
    {
        var result = true;
        foreach (var module in assembly.Modules)
        foreach (var type in module.Types)
        {
            var methodsSafe = CheckMethodsSecurity(type);
            var implementationsSafe = CheckProhibitedImplementations(type);
            
            result &= methodsSafe && implementationsSafe;
        }

        return result;
    }
    
    private bool CheckMethodsSecurity(TypeDefinition type)
    {
        foreach (var method in type.Methods)
        {
            if (!method.HasBody)
                continue;

            if (CheckIsNativeFunction(method))
                return false;
                    
            if (!CheckMethodReferences(method)) 
                return false;
        }

        return true;
    }

    private bool CheckMethodReferences(MethodDefinition method)
    {
        foreach (var instruction in method.Body.Instructions)
        {
            if (instruction.Operand is MethodReference methodRef)
            {
                var typeNamespace = methodRef.DeclaringType.Namespace;
                if (settingsProvider.Settings.ProhibitedNamespaces.Any(banned => typeNamespace.StartsWith(banned)))
                    return false;
            }
        }

        return true;
    }

    private static bool CheckIsNativeFunction(MethodDefinition method)
        => method.IsPInvokeImpl;

    private bool CheckProhibitedImplementations(TypeDefinition type)
        => !type.Interfaces.Any(x => _prohibitedInterfacesNames.Contains(x.InterfaceType.FullName));
}