using PluginInfrastructure.Normalization.Mappers;

namespace PluginInfrastructure;

public static class Normalizer
{
    private static readonly CrossPlatformPathMapper CrossPlatformPathMapper = new();
    
    public static string NormalizeUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));
    
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Only HTTP and HTTPS URLs are allowed", nameof(url));

        var resultUrl = uri.ToString().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return resultUrl.ToLowerInvariant();
    }

    public static string NormalizeDirectoryPath(string path)
    {
        var separator = Path.DirectorySeparatorChar;
        var processedPath = CrossPlatformPathMapper.MapToCurrentOS(path);
        var fullPath = Path.GetFullPath(processedPath.TrimEnd(separator, Path.AltDirectorySeparatorChar));
        
        return OperatingSystem.IsWindows() ? fullPath.ToLowerInvariant() : fullPath;
    }
}