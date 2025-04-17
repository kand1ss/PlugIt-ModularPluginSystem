namespace PluginInfrastructure;

public static class Normalizer
{
    public static string NormalizeUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));
    
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Only HTTP and HTTPS URLs are allowed", nameof(url));

        return uri.ToString();
    }

    public static string NormalizeDirectoryPath(string path)
        => Path.GetFullPath(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLowerInvariant();
}