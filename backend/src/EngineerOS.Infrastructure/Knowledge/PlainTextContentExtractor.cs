using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class PlainTextContentExtractor : IContentExtractor
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".md",
            ".txt",
            ".cs",
            ".ts",
            ".tsx",
            ".js",
            ".jsx",
            ".csproj",
            ".json",
            ".xml"
        };

    public bool CanExtract(string extension)
    {
        return SupportedExtensions.Contains(extension);
    }

    public async Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(
            filePath,
            cancellationToken);
    }
}