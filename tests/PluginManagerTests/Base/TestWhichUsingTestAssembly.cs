namespace PluginManagerTests.Base;

public class TestWhichUsingTestAssembly
{
    protected readonly string TestAssemblyPath;

    protected TestWhichUsingTestAssembly()
    {
        TestAssemblyPath = GetTestAssemblyPath();
    }
    
    protected static string GetTestAssemblyPath()
    {
        var current = Directory.GetCurrentDirectory();
        var solutionRoot = Directory.GetParent(current)!.Parent!.Parent!.Parent!.FullName;

        var relativePath = Path.Combine(
            solutionRoot,
            "TestAssembly",
            "bin",
            "Debug",
            "net9.0",
            "TestAssembly.dll"
        );

        return Path.GetFullPath(relativePath);
    }
}