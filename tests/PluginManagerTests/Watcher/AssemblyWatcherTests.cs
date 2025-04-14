using ModularPluginAPI.Components.AssemblyWatcher;
using PluginManagerTests.Base;
using Xunit;

namespace PluginManagerTests;

public class AssemblyWatcherTests : TestWhichUsingTestAssembly
{
    private readonly AssemblyWatcher _watcher = new();
    private readonly WatcherObserver _observer = new();
    
    private readonly string _tempFolderPath;
    private readonly string _tempAssemblyPath;

    public AssemblyWatcherTests()
    {
        _tempFolderPath = CreateTempFolder();
        _tempAssemblyPath = CopyAssemblyToTempFolder();
        _watcher.AddObserver(_observer);
    }

    ~AssemblyWatcherTests()
    {
        _watcher.RemoveObserver(_observer);
        File.Delete(_tempAssemblyPath);
    }

    private string CreateTempFolder()
    {
        var path = Path.Combine(Path.GetDirectoryName(TestAssemblyPath)!, "temp");
        Directory.CreateDirectory(path);
        return path;
    }

    private string CopyAssemblyToTempFolder()
    {
        var path = Path.Combine(_tempFolderPath, Path.GetFileName(TestAssemblyPath));
        File.Copy(TestAssemblyPath, path, true);
        return path;
    }

    private void RecreateTempAssembly()
    {
        File.Delete(_tempAssemblyPath);
        File.Create(_tempAssemblyPath).Close();
    }

    [Fact]
    public void ObserveAssembly_CorrectPath_AssemblyIsRegistered()
    {
        _watcher.ObserveAssembly(_tempAssemblyPath);

        RecreateTempAssembly();
        Thread.Sleep(100);

        Assert.Contains(_tempAssemblyPath, _observer.RemovedAssemblies);
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