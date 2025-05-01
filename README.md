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
PlugIt will **automatically** generate metadata for all assemblies(.dll), process it and detect all plugins. Registered assemblies will be tracked by the manager(if this is not disabled when the instance is created). Based on changes in the assembly, the manager will generate up-to-date metadata.


## 🧩 Creating a Plugin
To create a plugin, you can inherit from `PluginBase`. This base class provides built-in functionality, such as dependency support. In order to create a plugin that will interact with the network, you must inherit from `NetworkPluginBase`. If the plugin should interact with the file system - from `FilePluginBase`. Additionally, you can implement interfaces to extend your plugin's capabilities:

• **`IExecutingPlugin`** – allows the plugin to be executed.<br>
• **`IExtensionPlugin<T>`** – extends the functionality of specific types.<br>
• **`IConfigurablePlugin`** - provides JSON configuration file support for the plugin.(implemented by `PluginBase`)<br>
• **`IPluginWithDependencies`** - provides dependency support for other plugins. Implements `IConfigurablePlugin`. If you do not inherit from PluginBase, you must manually implement dependency loading and retrieval logic.(implemented by `PluginBase`)<br>

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
public class MyPlugin : IExecutingPlugin 
{
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

## 🛡️ Security System API
The Security System API provides robust mechanisms for ensuring the safe operation of plugins within the application environment. It offers:

• Validation and restriction of dangerous namespaces (e.g., System.IO, System.Reflection) to prevent unauthorized operations by plugins.<br>
• Management of file system and network access permissions through a centralized SecuritySettings configuration.<br>
• JSON-based configuration import, allowing administrators to easily manage and audit security policies.<br>
• Real-time checking of plugin actions against the assigned permissions, blocking any unauthorized attempts automatically.<br>

## 📂 Architecture
• PluginManager – the main facade that manages plugins<br>
• PluginDispatcher – coordinates loading and unloading<br>
• Services – handles plugin loading and metadata processing<br>
• Components – core and extra modules like loaders, validators, and loggers<br>

**Full architecture diagram:**

![plugIt-architecture drawio (1)](https://github.com/user-attachments/assets/9749c709-1c87-434b-9f35-3cf8e38c491f)
