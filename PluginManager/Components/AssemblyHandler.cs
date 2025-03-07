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

    public IEnumerable<IPlugin> GetAllPlugins(Assembly assembly)
        => assembly.GetTypes().Select(HandleType<IPlugin>).Where(p => p != null)!;
    
    public IEnumerable<IPlugin> GetAllPlugins(IEnumerable<Assembly> assemblies)
        => assemblies.SelectMany(GetAllPlugins);
    
    public IEnumerable<T> GetPlugins<T>(Assembly assembly) where T : class
        => assembly.GetTypes().Select(HandleType<T>).Where(p => p != null)!;
    
    public IEnumerable<T> GetPlugins<T>(IEnumerable<Assembly> assemblies) where T : class
        => assemblies.SelectMany(GetPlugins<T>);
}