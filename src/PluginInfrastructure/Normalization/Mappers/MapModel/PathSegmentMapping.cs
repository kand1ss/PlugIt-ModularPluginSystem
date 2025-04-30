namespace PluginInfrastructure.Normalization.Mappers.MapModel;

public class PathSegmentMapping(string windowsSegment, string linuxSegment)
{
    public string WindowsSegment { get; } = windowsSegment;
    public string LinuxSegment { get; } = linuxSegment;
}