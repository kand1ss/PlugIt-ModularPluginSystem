using PluginInfrastructure.Normalization.Mappers.MapModel;

namespace PluginInfrastructure.Normalization.Mappers;

public class CrossPlatformPathMapper
{
    private readonly List<PathSegmentMapping> _pathSegmentMaps = [
        new("Users", "home"),
        new("Users\\Administrator", "root"),
        new("Users\\Public", "usr/share/common"),
        new("AppData\\Local", ".local/share"),
        new("AppData\\Roaming", ".config"),
        new("ProgramData", "etc"),
        new("Program Files", "usr/bin"),
        new("Program Files (x86)", "usr/bin"),
        new("AppData\\Local\\Temp", ".cache/tmp"),
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
        if (CheckPathIsWindows(path) || string.IsNullOrEmpty(path))
            return path;

        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var convertedSegments = MatchCombinationsAndAddSegment(pathSegments,
            map => map.WindowsSegment,
            map => map.LinuxSegment.Split('/', StringSplitOptions.RemoveEmptyEntries),
            map => map.LinuxSegment.Length);

        var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C:";
        return systemDrive + "\\" + string.Join("\\", convertedSegments);
    }

    public string MapToUnix(string path)
    {
        if (CheckPathIsUnix(path) || string.IsNullOrEmpty(path))
            return path;
     
        string trimmedPath = TrimWindowsDriveLetter(path);

        var pathSegments = trimmedPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        var resultSegments = MatchCombinationsAndAddSegment(
            pathSegments, 
            map => map.LinuxSegment,
            map => map.WindowsSegment.Split('\\'),
            map => map.WindowsSegment.Length);
        
        var resultPath = string.Join("/", resultSegments);
        if (!resultPath.StartsWith("/"))
            resultPath = "/" + resultPath;
        
        return resultPath;
    }

    private static string TrimWindowsDriveLetter(string path)
    {
        return path.Length > 2 && path[1] == ':' && path[2] == '\\'
            ? path.Substring(3) : path;
    }
    
    private IEnumerable<string> MatchCombinationsAndAddSegment(
        string[] segments, 
        Func<PathSegmentMapping, string> mapToSegmentType, // тип сегмента в который преобразовываем исходный сегмент
        Func<PathSegmentMapping, string[]> splitStrategy, // каким образом сегментировать маппинг
        Func<PathSegmentMapping, int> sortPredicate) // по какому принципу отсортировать маппинг
    {
        var sortedPathsMap = 
            _pathSegmentMaps.OrderByDescending(sortPredicate).ToList();

        var resultSegments = new List<string>();
        int currentSegmentIndex = 0;
        
        while (currentSegmentIndex < segments.Length)
        {
            bool matchFound = false;
            foreach (var map in sortedPathsMap)
            {
                var mapSegments = splitStrategy(map);
                if (currentSegmentIndex + mapSegments.Length > segments.Length) 
                    continue;
                
                bool currentMatch = CheckMapSegments(segments, mapSegments, currentSegmentIndex);
                if (currentMatch)
                {
                    resultSegments.Add(mapToSegmentType(map));
                    currentSegmentIndex += mapSegments.Length;
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound)
            {
                resultSegments.Add(segments[currentSegmentIndex]);
                currentSegmentIndex++;
            }
        }

        return resultSegments;
    }

    private static bool CheckMapSegments(string[] segments, string[] mapSegments, int currentSegmentIndex)
    {
        bool currentMatch = true;
        for (int k = 0; k < mapSegments.Length; k++)
        {
            if (!segments[currentSegmentIndex + k].Equals(mapSegments[k], StringComparison.OrdinalIgnoreCase))
            {
                currentMatch = false;
                break;
            }
        }

        return currentMatch;
    }

    private bool CheckPathIsUnix(string path)
        => path.StartsWith("/");
    
    private bool CheckPathIsWindows(string path)
        => path.Length > 3 &&
           char.IsLetter(path[0]) &&
           path[1] == ':' &&
           path[2] == '\\';
}