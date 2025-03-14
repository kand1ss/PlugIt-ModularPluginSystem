using System.Reflection;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class AssemblyHandler : IAssemblyHandler
{
    private T? HandleType<T>(Type type) where T : class
    {
        if (!type.IsClass || type.IsAbstract || type.IsGenericType)
            return null;

        if (!typeof(T).IsAssignableFrom(type))
            return null;
        
        try
        {
            return Activator.CreateInstance(type) as T;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create an instance of {type.FullName}.", ex);
        }
    }
    
    public T? GetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
        => assembly.DefinedTypes.Select(HandleType<T>)
            .FirstOrDefault(plugin => plugin != null && plugin.Name == pluginName);

    public IEnumerable<IPlugin> GetAllPlugins(Assembly assembly)
        => assembly.DefinedTypes.Select(HandleType<IPlugin>).Where(p => p != null)!;
}