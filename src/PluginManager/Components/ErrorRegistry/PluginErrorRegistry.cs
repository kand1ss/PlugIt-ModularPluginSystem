using ModularPluginAPI.Components.ErrorRegistry.Interfaces;
using ModularPluginAPI.Components.ErrorRegistry.Models;
using ModularPluginAPI.Components.ErrorRegistry.Observer;
using ModularPluginAPI.Components.Lifecycle;
using ModularPluginAPI.Components.Observer;

namespace ModularPluginAPI.Components.ErrorRegistry;

public class PluginErrorRegistry : IPluginErrorRegistry, IErrorHandledPluginExecutorObserver
{
    private readonly Dictionary<string, HashSet<ErrorData>> _errors = new();
    private readonly List<IPluginErrorRegistryObserver> _observers = new();

    public void NotifyObservers(Action<IPluginErrorRegistryObserver> action)
    {
        foreach(var observer in _observers)
            action(observer);
    }
    
    public void AddObserver(IPluginErrorRegistryObserver observer)
        => _observers.Add(observer);

    public void RemoveObserver(IPluginErrorRegistryObserver observer)
        => _observers.Remove(observer);
    
    public void OnPluginFaulted(PluginInfo plugin, Exception exception)
    {
        var errorData = ErrorDataMapper.Map(plugin, exception);
        AddError(errorData);
    }

    private void AddError(ErrorData error)
    {
        if (_errors.TryGetValue(error.PluginName, out var errors))
            errors.Add(error);
        else
            _errors.Add(error.PluginName, [error]);
        
        NotifyObservers(a => a.OnErrorAdded(error));
    }

    private void RemoveErrors(string pluginName, IEnumerable<ErrorData> data)
    {
        if (!_errors.TryGetValue(pluginName, out var errors))
            return;

        foreach (var error in data)
        {
            errors.Remove(error);
            NotifyObservers(a => a.OnErrorRemoved(error));
        }
    }

    public void RemoveErrors(string pluginName)
    {
        if (_errors.TryGetValue(pluginName, out var errors))
            RemoveErrors(pluginName, errors);
    }

    
    public void RemoveErrorByException(string pluginName, Type exceptionType)
    {
        if (!_errors.TryGetValue(pluginName, out var errors))
            return;
        
        var exceptionErrors = errors.Where(e => e.ExceptionType == exceptionType);
        RemoveErrors(pluginName, exceptionErrors);
    }


    public IEnumerable<ErrorData> GetAllErrors()
        => _errors.Values.SelectMany(errors => errors);

    public IEnumerable<ErrorData> GetErrors(string pluginName)
    {
        if (!_errors.TryGetValue(pluginName, out var errors))
            return [];
        
        return errors;
    }
}