using ModularPluginAPI.Components.ErrorRegistry;
using TestAssembly;
using Xunit;

namespace PluginManagerTests;

public class PluginErrorRegistryTests
{
    private readonly PluginErrorRegistry _errorRegistry = new();
    private readonly ErrorObserverSubject _subject = new();
    private readonly FaultedPlugin _plugin = new();

    public PluginErrorRegistryTests()
    {
        _subject.AddObserver(_errorRegistry);
    }
    
    [Fact]
    public void AddError_ErrorAdded()
    {
        _subject.AddError(_plugin);
        var errors = _errorRegistry.GetAllErrors().ToList();
        
        Assert.Single(errors);
        Assert.Equal(_plugin.Name, errors.First().PluginName);
    }

    [Fact]
    public void EmptyRegistry_NoErrorsInRegistry()
    {
        Assert.Empty(_errorRegistry.GetAllErrors());
    }

    [Fact]
    public void GetErrors_CorrectPluginName_ReceivedCorrectErrors()
    {
        var secondPlugin = new FaultedExtensionPlugin();
        _subject.AddError(_plugin);
        _subject.AddError(_plugin);
        _subject.AddError(secondPlugin);

        var firstPluginErrors = _errorRegistry.GetErrors(_plugin.Name);
        var secondPluginErrors = _errorRegistry.GetErrors(secondPlugin.Name);

        Assert.Equal(2, firstPluginErrors.Count(x => x.PluginName == _plugin.Name));
        Assert.Single(secondPluginErrors, x => x.PluginName == secondPlugin.Name);
    }

    [Fact]
    public void GetErrors_FakePluginName_ReceivedEmptyListOfErrors()
        => Assert.Empty(_errorRegistry.GetErrors("SomeFakePlugin"));

    [Fact]
    public void RemoveErrors_CorrectPluginName_ErrorsRemoved()
    {
        _subject.AddError(_plugin);
        _errorRegistry.RemoveErrors(_plugin.Name);
        
        Assert.Empty(_errorRegistry.GetAllErrors());
        Assert.Empty(_errorRegistry.GetErrors(_plugin.Name));
    }

    [Fact]
    public void RemoveErrors_FakePluginName_NothingHappened()
    {
        _errorRegistry.RemoveErrors("SomeFakePlugin");
        Assert.Empty(_errorRegistry.GetAllErrors());
        Assert.Empty(_errorRegistry.GetErrors("SomeFakePlugin"));
    }

    [Fact]
    public void RemoveErrorsByException_CorrectPluginName_ErrorRemoved()
    {
        _subject.AddError(_plugin, new Exception());
        _subject.AddError(_plugin, new InvalidOperationException());
        
        _errorRegistry.RemoveErrorsByException(_plugin.Name, typeof(Exception));
        Assert.Single(_errorRegistry.GetAllErrors());
        Assert.Single(_errorRegistry.GetErrors(_plugin.Name));
    }

    [Fact]
    public void RemoveErrorsByException_DuplicateExceptionType_BothExceptionsRemoved()
    {
        _subject.AddError(_plugin, new InvalidOperationException());
        _subject.AddError(_plugin, new InvalidOperationException());
        
        _errorRegistry.RemoveErrorsByException(_plugin.Name, typeof(InvalidOperationException));
        Assert.Empty(_errorRegistry.GetAllErrors());
        Assert.Empty(_errorRegistry.GetErrors(_plugin.Name));
    }

    [Fact]
    public void RemoveErrorsByException_FakePluginName_NothingHappened()
    {
        _errorRegistry.RemoveErrorsByException(_plugin.Name, typeof(Exception));
        Assert.Empty(_errorRegistry.GetAllErrors());
        Assert.Empty(_errorRegistry.GetErrors(_plugin.Name));
    }
}