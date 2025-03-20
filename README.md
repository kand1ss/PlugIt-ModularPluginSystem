# PlugIt🔌 - ModularPluginSystem


## 📌 Description
PlugIt is a plugin management system designed for dynamically loading, unloading, and managing dependencies between modules in applications.

## 🚀 Features
✅ Dynamic loading and unloading of plugins<br>
✅ Dependency management between plugins<br>
✅ Flexible architecture with modular extensibility<br>
✅ Logging and monitoring of plugin states<br>

## 🚦 Quick Start
Create a plugins folder and place your plugin DLL files inside.<br>
Initialize PlugIt and provide the path to the plugins folder:<br>
```csharp
var pluginManager = new PluginManager("path/to/plugins");
```
PlugIt will **automatically** generate metadata for all assemblies(.dll), process it and detect all plugins.


## 🧩 Creating a Plugin
To create a plugin, you can inherit from `PluginBase`. This base class provides built-in functionality, such as dependency support. Additionally, you can implement interfaces to extend your plugin's capabilities:

• **`IInitialisablePlugin`** – provides initialization logic for plugins.(implemented by `PluginBase`)<br>
• **`IExecutingPlugin`** – allows the plugin to be executed.<br>
• **`IFinalisablePlugin`** – provides finalization logic for plugins.(implemented by `PluginBase`)<br>
• **`IConfigurablePlugin`** - provides JSON configuration file support for the plugin.(implemented by `PluginBase`)<br>
• **`IPluginWithDependencies`** - provides dependency support for other plugins. Implements `IConfigurablePlugin`. If you do not inherit from PluginBase, you must manually implement dependency loading and retrieval logic.<br>
• **`IExtensionPlugin<T>`** – extends the functionality of specific types.<br>
• **`INetworkPlugin`** – enables network interactions.<br>

### Example:
```csharp
public class MyPlugin : PluginBase, IExecutingPlugin 
{
	public void Initialize()
	{
		Console.WriteLine("Initializing");
	}
	public void Execute()
	{
		Console.WriteLine("My plugin is running!");
	}
	public void FinalizePlugin()
	{
		Console.WriteLine("Finalizing");
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

## 📂 Architecture
• PluginManager – the main facade that manages plugins<br>
• PluginDispatcher – coordinates loading and unloading<br>
• Services – handles plugin loading and metadata processing<br>
• Components – core and extra modules like loaders, validators, and loggers<br>

**Full architecture diagram:**

![plugIt-architecture](https://github.com/user-attachments/assets/e648c726-2f28-443c-86f1-144c9b8ad7d9)
