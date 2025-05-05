using Mono.Cecil;

namespace ModularPluginAPI.Services.Interfaces;

public interface IAssemblyAnalyzer
{
    bool Analyze(AssemblyDefinition assemblyDefinition);
}