using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Infrastructure.Storage;

public sealed class LocalRepositoryFileReader : IRepositoryFileReader
{
    private readonly string _storageRoot;

    public LocalRepositoryFileReader(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public Task<IReadOnlyList<string>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var extractedDirectory = Path.Combine(
            _storageRoot,
            "repositories",
            repositoryId.ToString(),
            "extracted");

        if (!Directory.Exists(extractedDirectory))
        {
            return Task.FromResult<IReadOnlyList<string>>([]);
        }

        var files = new List<string>();

        foreach (var fullPath in Directory.EnumerateFiles(
                     extractedDirectory,
                     "*",
                     SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var relativePath = Path.GetRelativePath(
                extractedDirectory,
                fullPath);

            files.Add(relativePath.Replace('\\', '/'));
        }

        files.Sort(StringComparer.Ordinal);

        return Task.FromResult<IReadOnlyList<string>>(files);
    }
}