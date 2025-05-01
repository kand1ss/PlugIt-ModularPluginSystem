using System.Reflection;
using System.Text.Json;
using ModularPluginAPI.Exceptions;
using PluginAPI;

namespace ModularPluginAPI.Components;

public class AssemblyHandler : IAssemblyHandler
{
    private static bool ValidateType<T>(Type type) where T : class, IPluginData
        => type.IsClass 
               && !type.IsAbstract 
               && !type.IsGenericType 
               && typeof(T).IsAssignableFrom(type) 
               && type.GetConstructor(Type.EmptyTypes) is not null;

    private static T? CreateInstance<T>(Type type) where T : class, IPluginData
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

    private static T? HandleType<T>(Type type) where T : class, IPluginData
        => ValidateType<T>(type) ? CreateInstance<T>(type) : null;
    private static IEnumerable<T> HandleTypes<T>(IEnumerable<Type> types) where T : class, IPluginData
        => types.Select(HandleType<T>).OfType<T>();


    private static string? GetConfigurationName(string[] allConfigurations, string configFileName)
        => allConfigurations.FirstOrDefault(x => x.EndsWith(configFileName));

    private static Stream? GetConfiguration(Assembly assembly, string? configFileName)
    {
        if (string.IsNullOrEmpty(configFileName))
            return null;
        
        return assembly.GetManifestResourceStream(configFileName);
    }

    private static T FindPlugin<T>(IEnumerable<T> plugins, string pluginName) where T : class, IPluginData
        => plugins.FirstOrDefault(p => p.Name == pluginName)
            ?? throw new PluginNotFoundException(pluginName);

    private static Stream? TryGetPluginConfiguration<T>(Assembly assembly, T plugin) where T : class, IPluginData
    {
        var allConfigurations = assembly.GetManifestResourceNames();
        var pluginName = plugin.GetType().FullName ?? "null";
        var configName = GetConfigurationName(allConfigurations, pluginName);

        return GetConfiguration(assembly, configName);
    }

    private static void TryLoadPluginConfiguration<T>(Stream? configurationStream, T plugin) where T : class, IPluginData
    {
        if (configurationStream is null || plugin is not IConfigurablePlugin configurable) 
            return;
        
        using var reader = new StreamReader(configurationStream);
        var json = reader.ReadToEnd();
        
        try
        {
            var config = JsonSerializer.Deserialize<PluginConfiguration>(json);
            configurable.LoadConfiguration(config);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException($"Invalid JSON configuration for plugin {plugin.Name}.");
        }
    }


    private IEnumerable<T> GetPlugins<T>(Assembly assembly) where T : class, IPluginData
    {
        var assemblyTypes = assembly.DefinedTypes;
        return HandleTypes<T>(assemblyTypes);
    }

    private void GetAndLoadPluginConfiguration(Assembly assembly, IPluginData plugin)
    {
        var pluginConfiguration = TryGetPluginConfiguration(assembly, plugin);
        TryLoadPluginConfiguration(pluginConfiguration, plugin);
    }

    public T GetPlugin<T>(Assembly assembly, string pluginName) where T : class, IPluginData
    {
        var pluginsFromAssembly = GetPlugins<T>(assembly);
        var plugin = FindPlugin(pluginsFromAssembly, pluginName);
        
        GetAndLoadPluginConfiguration(assembly, plugin);
        return plugin;
    }

    public IEnumerable<IPluginData> GetAllPlugins(Assembly assembly)
    {
        var plugins = GetPlugins<IPluginData>(assembly).ToList();
        foreach (var plugin in plugins)
            GetAndLoadPluginConfiguration(assembly, plugin);
        
        return plugins;
    }
}