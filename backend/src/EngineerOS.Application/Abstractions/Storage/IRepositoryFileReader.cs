namespace EngineerOS.Application.Abstractions.Storage;

public interface IRepositoryFileReader
{
    Task<IReadOnlyList<RepositoryFile>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}
