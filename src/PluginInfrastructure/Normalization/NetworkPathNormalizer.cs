namespace PluginInfrastructure.Normalization;

public class NetworkPathNormalizer : IPathNormalizer
{
    public string Normalize(string path)
        => Normalizer.NormalizeUrl(path);
}