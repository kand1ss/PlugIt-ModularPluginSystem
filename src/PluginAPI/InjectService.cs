
namespace PluginAPI;

public static class InjectService<T>
{
    public static T TryInject(T item, ref bool flagIsInjected)
    {
        if (flagIsInjected || item is null)
            throw new InvalidOperationException("Failed attempt to inject service.");

        flagIsInjected = true;
        return item;
    }
}