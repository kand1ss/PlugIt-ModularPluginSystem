using ModularPluginAPI.Components.ErrorRegistry.Models;
using ModularPluginAPI.Components.ErrorRegistry.Observer;

namespace ModularPluginAPI.Components.ErrorRegistry.Interfaces;

/// <summary>
/// Defines methods for managing and observing error data for plugins.
/// </summary>
public interface IPluginErrorRegistry : IObservablePluginErrorRegistry
{
    /// <summary>
    /// Retrieves all error data stored in the registry.
    /// </summary>
    /// <returns>
    /// An enumerable collection of all <see cref="ErrorData"/> instances recorded in the registry.
    /// </returns>
    IEnumerable<ErrorData> GetAllErrors();

    /// <summary>
    /// Retrieves error data associated with a specific plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose errors are to be retrieved.</param>
    /// <returns>
    /// An enumerable collection of <see cref="ErrorData"/> instances related to the specified plugin.
    /// </returns>
    IEnumerable<ErrorData> GetErrors(string pluginName);

    /// <summary>
    /// Removes all error data associated with the specified plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose errors should be removed.</param>
    void RemoveErrors(string pluginName);

    /// <summary>
    /// Removes error data associated with the specified plugin that match the given exception type.
    /// </summary>
    /// <param name="pluginName">The name of the plugin for which the errors should be removed.</param>
    /// <param name="exceptionType">The type of the exception whose corresponding error entries should be removed.</param>
    void RemoveErrorByException(string pluginName, Type exceptionType);
}
