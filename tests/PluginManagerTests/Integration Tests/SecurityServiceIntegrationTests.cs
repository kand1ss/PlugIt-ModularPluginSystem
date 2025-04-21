using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;
using Moq;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests.Integration_Tests;

public class SecurityServiceIntegrationTests : TestWhichUsingTestAssembly
{
    private readonly SecurityService _securityService;
    private AssemblyMetadata _assemblyMetadata;
    
    private readonly string _unsafeAssemblyPath;

    public SecurityServiceIntegrationTests()
    {
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        var loaderMock = new Mock<IAssemblyLoader>();
        var handlerMock = new Mock<IAssemblyHandler>();
        
        _securityService = new SecurityService(loaderMock.Object, handlerMock.Object, logger);
        _assemblyMetadata = InitializeAssemblyMetadata();
        _unsafeAssemblyPath = GetUnsafeAssemblyPath();
    }

    private AssemblyMetadata InitializeAssemblyMetadata()
    {
        var assemblyMetadata = new AssemblyMetadata();
        assemblyMetadata.Name = "SomeAssembly";
        assemblyMetadata.Version = new(1,0,0);
        assemblyMetadata.Path = TestAssemblyPath;
        
        var pluginMetadata = new PluginMetadata
        {
            Name = "SomePlugin",
        };
        pluginMetadata.Configuration.Permissions.FileSystemPaths.Add(TestAssemblyPath);
        
        assemblyMetadata.Plugins.Add(pluginMetadata);
        return assemblyMetadata;
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
    

    [Fact]
    public void CheckSafety_SafetyAssembly_ReturnsTrue()
    {
        _securityService.AddFileSystemPermission(TestAssemblyPath);
        _securityService.AddBlockedNamespace("System.IO");
        
        Assert.True(_securityService.CheckSafety(_assemblyMetadata));
    }

    [Fact]
    public void CheckSafety_SafetyFilePathNotSafetyNamespace_ReturnsFalse()
    {
        _assemblyMetadata.Path = _unsafeAssemblyPath;
        _securityService.AddBlockedNamespace("System.IO");

        Assert.False(_securityService.CheckSafety(_assemblyMetadata));
    }

    [Fact]
    public void CheckSafety_SafetyNamespaceNotSafetyFilePath_ReturnsFalse()
    {
        _securityService.AddFileSystemPermission(_unsafeAssemblyPath);

        Assert.False(_securityService.CheckSafety(_assemblyMetadata));
    }
}