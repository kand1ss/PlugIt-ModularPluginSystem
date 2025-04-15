using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Logger;
using Moq;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblySecurityServiceTests : TestWhichUsingTestAssembly
{
    private readonly AssemblySecurityService _service;

    private readonly string _unsafeAssemblyPath;
    private readonly string _safeAssemblyPath;

    public AssemblySecurityServiceTests()
    {
        _unsafeAssemblyPath = GetUnsafeAssemblyPath();
        _safeAssemblyPath = GetSafeAssemblyPath();

        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        _service = new AssemblySecurityService();
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
    public void AddBlockedNamespace_CorrectNamespace_ReturnsTrue()
    {
        Assert.True(_service.AddBlockedNamespace("System"));
    }

    [Fact]
    public void AddBlockedNamespace_IncorrectNamespace_ReturnsFalse()
    {
        Assert.False(_service.AddBlockedNamespace("System-IO"));
        Assert.False(_service.AddBlockedNamespace("System_IO"));
        Assert.False(_service.AddBlockedNamespace("System.Collections_Generic"));
        Assert.False(_service.AddBlockedNamespace("123System"));
        Assert.False(_service.AddBlockedNamespace("System123"));
    }

    [Fact]
    public void RemoveBlockedNamespace_CorrectNamespace_ReturnsTrue()
    {
        _service.AddBlockedNamespace("System");
        Assert.True(_service.RemoveBlockedNamespace("System"));
    }

    [Fact]
    public void RemoveBlockedNamespace_FakeNamespace_ReturnsFalse()
    {
        Assert.False(_service.RemoveBlockedNamespace("System.Namespace"));
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
        _service.AddBlockedNamespace("System");
        Assert.False(_service.CheckSafety(_safeAssemblyPath));
    }

    [Fact]
    public void CheckSafety_UnsafeAssemblyNowSafe_ReturnsTrue()
    {
        _service.RemoveBlockedNamespace("System.IO");
        Assert.True(_service.CheckSafety(_unsafeAssemblyPath));
    }
}