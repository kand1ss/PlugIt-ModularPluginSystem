namespace PluginInfrastructure.Normalization;

public class FilePathNormalizer : IPathNormalizer
{
    public string Normalize(string path)
        => Normalizer.NormalizeDirectoryPath(path);
}