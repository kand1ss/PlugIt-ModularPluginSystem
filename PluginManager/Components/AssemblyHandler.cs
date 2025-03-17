using System.Reflection;
using System.Text.Json;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class AssemblyHandler : IAssemblyHandler
{
    private static bool ValidateType<T>(Type type) where T : class, IPlugin
        => type.IsClass 
               && !type.IsAbstract 
               && !type.IsGenericType 
               && typeof(T).IsAssignableFrom(type) 
               && type.GetConstructor(Type.EmptyTypes) is not null;

    private static T? CreateInstance<T>(Type type) where T : class, IPlugin
    {
        try
        {
            return Activator.CreateInstance(type) as T;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create an instance of {type.FullName}.", ex);
        }
    }

    private static T? HandleType<T>(Type type) where T : class, IPlugin
        => ValidateType<T>(type) ? CreateInstance<T>(type) : null;

    
    
    private static void TryLoadPluginConfiguration<T>(Stream? configurationStream, T plugin) where T : class, IPlugin
    {
        if (configurationStream is null || plugin is not IConfigurablePlugin configurable) 
            return;
        
        using var reader = new StreamReader(configurationStream);
        var json = reader.ReadToEnd();
        var config = JsonSerializer.Deserialize<PluginConfiguration>(json);
        configurable.LoadConfiguration(config);
    }
    
    private static Stream? TryGetPluginConfiguration<T>(Assembly assembly, T plugin) where T : class, IPlugin
    {
        var allConfigurations = assembly.GetManifestResourceNames();
        var configFileName = plugin.GetType().FullName ?? "Unknown";
        var configResource = allConfigurations.SingleOrDefault(x => x.EndsWith(configFileName));

        return configResource is not null ? assembly.GetManifestResourceStream(configResource) : null;
    }
    
    
    
    public T? GetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPlugin
    {
        var assemblyTypes = assembly.DefinedTypes;
        var pluginsFromAssembly = assemblyTypes.Select(HandleType<T>);
        
        var plugin = pluginsFromAssembly.FirstOrDefault(p => p is not null && p.Name == pluginName);
        if (plugin is null)
            return null;

        var configurationStream = TryGetPluginConfiguration(assembly, plugin);
        TryLoadPluginConfiguration(configurationStream, plugin);
        
        return plugin;
    }

    public IEnumerable<IPlugin> GetAllPlugins(Assembly assembly)
    {
        var assemblyTypes = assembly.DefinedTypes;
        var plugins = assemblyTypes.Select(HandleType<IPlugin>).OfType<IPlugin>().ToList();

        foreach (var plugin in plugins)
        {
            var pluginConfiguration = TryGetPluginConfiguration(assembly, plugin);
            TryLoadPluginConfiguration(pluginConfiguration, plugin);
        }
        
        return plugins;
    }
}