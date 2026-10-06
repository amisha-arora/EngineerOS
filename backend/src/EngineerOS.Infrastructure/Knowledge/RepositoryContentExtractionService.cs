using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Abstractions.RepositoryFiles;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class RepositoryContentExtractionService
    : IRepositoryContentExtractionService
{
    private readonly IRepositoryFileReader _fileReader;
    private readonly IContentExtractorResolver _extractorResolver;
    private readonly string _storageRoot;

    public RepositoryContentExtractionService(
        IRepositoryFileReader fileReader,
        IContentExtractorResolver extractorResolver,
        string storageRoot)
    {
        _fileReader = fileReader;
        _extractorResolver = extractorResolver;
        _storageRoot = storageRoot;
    }

    // Existing ID-based extraction
    public async Task<string> ExtractAsync(
        Guid repositoryId,
        Guid repositoryFileId,
        CancellationToken cancellationToken = default)
    {
        var files = await _fileReader.GetFilesAsync(
            repositoryId,
            cancellationToken);

        var file = files.FirstOrDefault(
            file => file.Id == repositoryFileId);

        if (file is null)
        {
            throw new InvalidOperationException(
                $"Repository file '{repositoryFileId}' was not found.");
        }

        return await ExtractAsync(
            repositoryId,
            file.Path,
            file.Extension,
            cancellationToken);
    }

    // New path-based extraction
    public async Task<string> ExtractAsync(
        Guid repositoryId,
        string relativePath,
        string extension,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException(
                "Relative path cannot be empty.",
                nameof(relativePath));
        }

        var extractedDirectory = Path.Combine(
            _storageRoot,
            "repositories",
            repositoryId.ToString(),
            "extracted");

        var normalizedRelativePath = relativePath
            .Replace(
                '/',
                Path.DirectorySeparatorChar)
            .Replace(
                '\\',
                Path.DirectorySeparatorChar);

        var fullPath = Path.Combine(
            extractedDirectory,
            normalizedRelativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"Repository file '{relativePath}' was not found on disk.",
                fullPath);
        }

        var extractor =
            _extractorResolver.Resolve(extension);

        return await extractor.ExtractAsync(
            fullPath,
            cancellationToken);
    }
}