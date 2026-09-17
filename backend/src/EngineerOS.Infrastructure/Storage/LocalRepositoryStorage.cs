using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Infrastructure.Storage;

public sealed class LocalRepositoryStorage
    : IRepositoryStorage
{
    private readonly string _storageRoot;

    public LocalRepositoryStorage(
        string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public async Task<string> SaveAsync(
        Guid repositoryId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var repositoryDirectory =
            Path.Combine(
                _storageRoot,
                "repositories",
                repositoryId.ToString());

        Directory.CreateDirectory(
            repositoryDirectory);

        var filePath =
            Path.Combine(
                repositoryDirectory,
                "repository.zip");

        await using var outputStream =
            new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

        await fileStream.CopyToAsync(
            outputStream,
            cancellationToken);

        return filePath;
    }
}