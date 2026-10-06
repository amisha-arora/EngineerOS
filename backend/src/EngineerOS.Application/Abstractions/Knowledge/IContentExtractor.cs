namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IContentExtractor
{
    bool CanExtract(string extension);

    Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}