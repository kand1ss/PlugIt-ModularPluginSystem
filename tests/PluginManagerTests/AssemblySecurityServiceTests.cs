using ModularPluginAPI.Components;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblySecurityServiceTests : TestWhichUsingTestAssembly
{
    private readonly AssemblySecurityService _service;
    private readonly SecuritySettingsProvider _settingsProvider = new();
    private SecuritySettings _settings => _settingsProvider.Settings;

    private readonly string _unsafeAssemblyPath;
    private readonly string _safeAssemblyPath;

    public AssemblySecurityServiceTests()
    {
        _unsafeAssemblyPath = GetUnsafeAssemblyPath();
        _safeAssemblyPath = GetSafeAssemblyPath();

        _service = new AssemblySecurityService(_settingsProvider);
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
    public void CheckSafety_UnsafeAssembly_ReturnsFalse()
    {
        Assert.False(_service.CheckSafety(_unsafeAssemblyPath));
    }

    [Fact]
    public void CheckSafety_SafeAssembly_ReturnsTrue()
    {
        Assert.True(_service.CheckSafety(_safeAssemblyPath));
    }

    [Fact]
    public void CheckSafety_SafeAssemblyNowUnsafe_ReturnsFalse()
    {
        _settings.AddBlockedNamespace("System");
        Assert.False(_service.CheckSafety(_safeAssemblyPath));
    }

    [Fact]
    public void CheckSafety_UnsafeAssemblyNowSafe_ReturnsTrue()
    {
        _settings.RemoveBlockedNamespace("System.IO");
        Assert.True(_service.CheckSafety(_unsafeAssemblyPath));
    }
}