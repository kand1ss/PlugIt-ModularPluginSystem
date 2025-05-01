using ModularPluginAPI.Components;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Logger;
using ModularPluginAPI.Models;
using Moq;
using Xunit;

namespace PluginManagerTests.Plugin_Tracker;

public class PluginTrackerTests
{
    private readonly PluginTracker _pluginTracker;
    private readonly TrackerObserverSubject _observer = new();

    private AssemblyMetadata _testMetadata;
    private AssemblyMetadata _secondTestMetadata;

    public PluginTrackerTests()
    {
        var loggerMock = new Mock<ILoggerService>();
        var logger = new PluginLoggingFacade(loggerMock.Object);
        
        _pluginTracker = new PluginTracker(logger);
        _observer.AddObserver(_pluginTracker);

        _testMetadata = new()
        {
            Name = "Test",
            Path = Path.Combine("D", "FakePath"),
            Plugins = new()
            {
                new PluginMetadata { Name = "Plugin1" },
                new PluginMetadata { Name = "Plugin2" }
            }
        };
        
        _secondTestMetadata = new()
        {
            Name = "Test2",
            Path = Path.Combine("D", "FakePath"),
            Plugins = new()
            {
                new PluginMetadata { Name = "Plugin3" },
                new PluginMetadata { Name = "Plugin4" }
            }
        };
    }

    [Fact]
    public void RegisterPlugins_CorrectMetadata_PluginsAdded()
    {
        _observer.AddMetadata(_testMetadata);
        
        Assert.NotEmpty(_pluginTracker.GetPluginsStatus());
        Assert.Equal(_testMetadata.Plugins.Count, _pluginTracker.GetPluginsStatus().Count());
        Assert.All(_pluginTracker.GetPluginsStatus().Select(x => x.Name), 
            name => Assert.Contains(name, _testMetadata.Plugins.Select(x => x.Name)));
    }

    [Fact]
    public void RegisterPlugins_AlreadyUsedMetadata_PluginDuplicatesNotAdded()
    {
        _observer.AddMetadata(_testMetadata);
        _observer.AddMetadata(_testMetadata);
        
        Assert.NotEmpty(_pluginTracker.GetPluginsStatus());
        Assert.Equal(_testMetadata.Plugins.Count, _pluginTracker.GetPluginsStatus().Count());
        Assert.All(_pluginTracker.GetPluginsStatus().Select(x => x.Name), 
            name => Assert.Contains(name, _testMetadata.Plugins.Select(x => x.Name)));
    }

    [Fact]
    public void RegisterPlugins_PluginHasUnloadedState()
    {
        _observer.AddMetadata(_testMetadata);
        
        Assert.Equal(_testMetadata.Plugins.Count, 
            _pluginTracker.GetPluginsStatus().Count(x => x.CurrentState == PluginState.Unloaded));
    }

    [Fact]
    public void RemovePlugins_CorrectMetadata_PluginsRemoved()
    {
        _observer.AddMetadata(_testMetadata);
        _observer.RemoveMetadata(_testMetadata);
        
        Assert.Empty(_pluginTracker.GetPluginsStatus());
    }

    [Fact]
    public void RemovePlugins_TwoMetadataOneRemoved_PluginsFromMetadataRemoved()
    {
        _observer.AddMetadata(_testMetadata);
        _observer.AddMetadata(_secondTestMetadata);
        
        _observer.RemoveMetadata(_testMetadata);
        
        Assert.NotEmpty(_pluginTracker.GetPluginsStatus());
        Assert.Equal(_secondTestMetadata.Plugins.Count, _pluginTracker.GetPluginsStatus().Count());
        Assert.All(_pluginTracker.GetPluginsStatus().Select(x => x.Name), 
            name => Assert.Contains(name, _secondTestMetadata.Plugins.Select(x => x.Name)));
    }

    [Fact]
    public void SetPluginState_CorrectPlugin_StateUpdated()
    {
        _observer.AddMetadata(_testMetadata);
        
        var pluginName = _testMetadata.Plugins.First().Name;
        
        _pluginTracker.SetPluginStatus(pluginName, PluginState.Loaded, PluginMode.Fixed);
        Assert.Equal(PluginState.Loaded, _pluginTracker.GetPluginStatus(pluginName).CurrentState);
    }

    [Fact]
    public void SetPluginState_FakePluginName_NothingChanged()
    {
        _pluginTracker.SetPluginStatus("FakeName", PluginState.Loaded, PluginMode.Fixed);
        Assert.Empty(_pluginTracker.GetPluginsStatus());
    }
}