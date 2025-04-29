using PluginInfrastructure.Normalization.Mappers.MapModel;

namespace PluginInfrastructure.Normalization.Mappers;

public class CrossPlatformPathMapper
{
    private readonly List<PathSegmentMapping> _pathSegmentMaps = [
        new("Users", "home"),
        new("Administrator", "root"),
        new("Program Files", "usr/bin"),
        new("Program Files (x86)", "usr/bin"),
        new("Temp", "tmp"),
    ];
    
    public string MapToCurrentOS(string path)
    {
        if (OperatingSystem.IsWindows())
            return MapToWindows(path);
        if (OperatingSystem.IsLinux())
            return MapToUnix(path);
        
        return path;
    }
    
    public string MapToWindows(string path)
    {
        if (!CheckPathIsUnix(path) || CheckPathIsWindows(path))
            return path;

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var resultSegments = new List<string>();
        int i = 0;

        while (i < segments.Length)
        {
            bool matched = false;
            foreach (var map in _pathSegmentMaps)
            {
                var linuxParts = map.LinuxSegment.Split('/');
                if (i + linuxParts.Length > segments.Length)
                    continue;

                bool isMatch = true;
                for (int j = 0; j < linuxParts.Length; j++)
                {
                    if (!segments[i + j].Equals(linuxParts[j], StringComparison.OrdinalIgnoreCase))
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    resultSegments.Add(map.WindowsSegment);
                    i += linuxParts.Length;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                resultSegments.Add(segments[i]);
                i++;
            }
        }

        var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        return systemDrive + "\\" + string.Join("\\", resultSegments);
    }



    public string MapToUnix(string path)
    {
        if (!CheckPathIsWindows(path) || CheckPathIsUnix(path))
            return path;
     
        string trimmedPath = path.Length > 2 && path[1] == ':' && path[2] == '\\'
            ? path.Substring(3) : path;

        var segments = trimmedPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return "/";
    
        for (int i = 0; i < segments.Length; i++)
        {
            var mappedSegment = _pathSegmentMaps
                .FirstOrDefault(x => x.WindowsSegment.Equals(segments[i], StringComparison.OrdinalIgnoreCase));
            
            if (mappedSegment != null)
                segments[i] = mappedSegment.LinuxSegment;
        }
    
        return "/" + string.Join("/", segments);
    }
    
    private bool CheckPathIsUnix(string path)
        => path.StartsWith("/");
    private bool CheckPathIsWindows(string path)
        => path.Length > 3 &&
           char.IsLetter(path[0]) &&
           path[1] == ':' &&
           path[2] == '\\';
}