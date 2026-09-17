namespace EngineerOS.Application.Abstractions.Storage;

public interface IRepositoryFileReader
{
    Task<IReadOnlyList<string>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}