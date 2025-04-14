# PlugIt🔌 - ModularPluginSystem


## 📌 Description
PlugIt is a plugin management system designed for dynamically loading, unloading, and managing dependencies between modules in applications.

## 🚀 Features
✅ Dynamic loading and unloading of plugins<br>
✅ Dynamic tracking of registered assemblies<br>
✅ Dependency management between plugins<br>
✅ Flexible architecture with modular extensibility<br>
✅ Logging divided by abstraction levels<br>
✅ **PluginTracker API**: Track plugin states based on generated metadata and integrate custom components via the `Observer` pattern<br>
✅ **Error Registry**: Monitor errors in plugins and integrate your own components to handle them (Optional)<br>
✅ **Security system**: Enforce custom security policies to block unwanted assemblies (Optional)<br>
✅ **Plugin Performance Profiler**: Profile plugin performance across all lifecycle stages (Optional)<br>

## 🚦 Quick Start
Create an instance of PluginManager.<br>
Use the `RegisterAssembly()` methods for a single assembly, or `RegisterAssembliesFromDirectory()` for assemblies within a directory, to register the assemblies in the manager.
```csharp
var pluginManager = new PluginManager();
pluginManager.RegisterAssembliesFromDirectory("C:\PathToFolder");
```
PlugIt will **automatically** generate metadata for all assemblies(.dll), process it and detect all plugins. Registered assemblies will be tracked by the manager. Based on changes in the assembly, the manager will generate up-to-date metadata.


## 🧩 Creating a Plugin
To create a plugin, you can inherit from `PluginBase`. This base class provides built-in functionality, such as dependency support. Additionally, you can implement interfaces to extend your plugin's capabilities:

• **`IInitialisablePlugin`** – provides initialization logic for plugins.<br>
• **`IExecutingPlugin`** – allows the plugin to be executed.<br>
• **`IFinalisablePlugin`** – provides finalization logic for plugins.<br>
• **`IConfigurablePlugin`** - provides JSON configuration file support for the plugin.(implemented by `PluginBase`)<br>
• **`IPluginWithDependencies`** - provides dependency support for other plugins. Implements `IConfigurablePlugin`. If you do not inherit from PluginBase, you must manually implement dependency loading and retrieval logic.<br>
• **`IExtensionPlugin<T>`** – extends the functionality of specific types.<br>
• **`INetworkPlugin`** – enables network interactions.<br>

### Example:
```csharp
public class MyPlugin : PluginBase, IExecutingPlugin 
{
	public void Execute()
	{
		Console.WriteLine("My plugin is running!");
	}
}
```
<br>It is not mandatory to inherit from PluginBase. You can implement only the necessary interfaces if you don't need the additional functionality provided by the base class:<br>
```csharp
public class MyPlugin : IInitialisablePlugin, IExecutingPlugin 
{
	public void Initialize()
	{
		Console.WriteLine("Initializing");
	}
	public void Execute()
	{
		Console.WriteLine("My plugin is running!");
	}
}
```

## 🔍 PluginTracker API
The **PluginTracker API** allows you to monitor plugin states based on the metadata generated during the registration process. It provides:

• Real-time tracking of plugin state changes (e.g., registration, removal, status updates)<br>
• The ability to integrate your own custom components that can subscribe to these events via the `Observer` pattern<br>
• This means that when a plugin’s state changes, all subscribed components are notified immediately, enabling you to implement custom reactions or logging.<br>

## 📂 Architecture
• PluginManager – the main facade that manages plugins<br>
• PluginDispatcher – coordinates loading and unloading<br>
• Services – handles plugin loading and metadata processing<br>
• Components – core and extra modules like loaders, validators, and loggers<br>

**Full architecture diagram:**

![plugIt-architecture drawio](https://github.com/user-attachments/assets/a52a2adc-df92-4f66-94ab-a2d988bdeae3)

