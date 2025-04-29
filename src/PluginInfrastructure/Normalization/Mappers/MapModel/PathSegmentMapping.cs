namespace PluginInfrastructure.Normalization.Mappers.MapModel;

public class PathSegmentMapping
{
    public string WindowsSegment { get; }
    public string LinuxSegment { get; }

    public PathSegmentMapping(string windowsSegment, string linuxSegment)
    {
        WindowsSegment = windowsSegment;
        LinuxSegment = linuxSegment;
    }
}