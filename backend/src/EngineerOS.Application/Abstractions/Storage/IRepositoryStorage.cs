namespace EngineerOS.Application.Abstractions.Storage;

public interface IRepositoryStorage
{
    Task<string> SaveAsync(
        Guid repositoryId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken);
}