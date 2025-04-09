using ModularPluginAPI.Components.AssemblyWatcher;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyWatcherTests : TestWhichUsingTestAssembly
{
    private readonly AssemblyWatcher _watcher = new();
    private readonly WatcherObserver _observer = new();
    
    private string _tempFolderPath;
    private string _tempAssemblyPath;

    public AssemblyWatcherTests()
    {
        CreateTempFolder();
        CopyAssemblyToTempFolder();
        _watcher.AddObserver(_observer);
    }

    private void CreateTempFolder()
    {
        _tempFolderPath = Path.Combine(Path.GetDirectoryName(TestAssemblyPath)!, "temp");
        Directory.CreateDirectory(_tempFolderPath);
    }

    private void CopyAssemblyToTempFolder()
    {
        _tempAssemblyPath = Path.Combine(_tempFolderPath, Path.GetFileName(TestAssemblyPath));
        File.Copy(TestAssemblyPath, _tempAssemblyPath, true);
    }

    private void RecreateTempAssembly()
    {
        File.Delete(_tempAssemblyPath);
        File.Copy(TestAssemblyPath, _tempAssemblyPath, true);
    }

    [Fact]
    public void ObserveAssembly_CorrectPath_AssemblyIsRegistered()
    {
        _watcher.ObserveAssembly(_tempAssemblyPath);

        RecreateTempAssembly();

        Assert.Contains(_tempAssemblyPath, _observer.RemovedAssemblies);
        Assert.NotEmpty(_observer.AddedAssemblies);
        Assert.Contains(_tempAssemblyPath, _observer.AddedAssemblies);
    }


    [Fact]
    public void ObserveAssembly_FakePath_AssemblyNotRegistered()
    {
        _watcher.ObserveAssembly(Path.Combine("D", "FakeAssembly.dll"));
        Assert.Empty(_observer.AddedAssemblies);
    }


    [Fact]
    public void UnobserveAssembly_CorrectPath_AssemblyIsUnregistered()
    {
        _watcher.ObserveAssembly(_tempAssemblyPath);
        _watcher.UnobserveAssembly(_tempAssemblyPath);
        
        RecreateTempAssembly();
        
        Assert.Empty(_observer.RemovedAssemblies);
        Assert.Empty(_observer.AddedAssemblies);
    }
}