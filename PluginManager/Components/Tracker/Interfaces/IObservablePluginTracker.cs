using ModularPluginAPI.Components.Lifecycle.Observer;

namespace ModularPluginAPI.Components;

public interface IObservablePluginTracker
{
    /// <summary>
    /// Adds an observer to receive notifications about plugin-related events.
    /// </summary>
    /// <param name="observer">
    /// An instance of <see cref="IPluginTrackerObserver"/> that will be notified about plugin registration, removal, and state changes.
    /// </param>
    /// <remarks>
    /// After adding the observer, it will start receiving updates about plugin lifecycle events.
    /// </remarks>
    void AddObserver(IPluginTrackerObserver observer);

    /// <summary>
    /// Removes an observer from the notification list.
    /// </summary>
    /// <param name="observer">
    /// An instance of <see cref="IPluginTrackerObserver"/> that will no longer receive notifications.
    /// </param>
    /// <remarks>
    /// Once removed, the observer will stop receiving updates about plugin events.
    /// </remarks>
    public void RemoveObserver(IPluginTrackerObserver observer);
}