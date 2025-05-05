using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Modules;
using Mono.Cecil;
using Xunit;

namespace PluginManagerTests;

public class AssemblyAnalyzerTests
{
    private readonly AssemblyAnalyzer _analyzer;
    private readonly SecuritySettingsProvider _settingsProvider = new();
    private SecuritySettings _settings => _settingsProvider.Settings;

    private readonly string _unsafeAssemblyPath;
    private readonly string _safeAssemblyPath;

    public AssemblyAnalyzerTests()
    {
        _unsafeAssemblyPath = GetUnsafeAssemblyPath();
        _safeAssemblyPath = GetSafeAssemblyPath();

        _analyzer = new AssemblyAnalyzer(_settingsProvider);
    }

    private string GetSolutionDirectory()
    {
        var current = Directory.GetCurrentDirectory();
        return Directory.GetParent(current)!.Parent!.Parent!.Parent!.FullName;
    }

    private string GetUnsafeAssemblyPath()
    {
        var solutionRoot = GetSolutionDirectory();
        var relativePath = Path.Combine(
            solutionRoot,
            "TestUnsafeAssembly",
            "bin",
            "Debug",
            "net8.0",
            "TestUnsafeAssembly.dll"
        );
        return Path.GetFullPath(relativePath);
    }

    private string GetSafeAssemblyPath()
    {
        var solutionRoot = GetSolutionDirectory();
        var relativePath = Path.Combine(
            solutionRoot,
            "TestSafeAssembly",
            "bin",
            "Debug",
            "net8.0",
            "TestSafeAssembly.dll"
        );
        return Path.GetFullPath(relativePath);
    }

    [Fact]
    public void Analyze_UnsafeAssembly_ReturnsFalse()
    {
        var assembly = AssemblyDefinition.ReadAssembly(_unsafeAssemblyPath);
        Assert.False(_analyzer.Analyze(assembly));
    }

    [Fact]
    public void Analyze_SafeAssembly_ReturnsTrue()
    {
        var assembly = AssemblyDefinition.ReadAssembly(_safeAssemblyPath);
        Assert.True(_analyzer.Analyze(assembly));
    }

    [Fact]
    public void Analyze_SafeAssemblyNowUnsafe_ReturnsFalse()
    {
        _settings.AddBlockedNamespace("System");
        var assembly = AssemblyDefinition.ReadAssembly(_safeAssemblyPath);
        
        Assert.False(_analyzer.Analyze(assembly));
    }

    [Fact]
    public void Analyze_UnsafeAssemblyNowSafe_ReturnsTrue()
    {
        _settings.RemoveBlockedNamespace("System.IO");
        var assembly = AssemblyDefinition.ReadAssembly(_unsafeAssemblyPath);
        
        Assert.True(_analyzer.Analyze(assembly));
    }
}