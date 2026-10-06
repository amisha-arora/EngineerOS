namespace EngineerOS.Application.Abstractions.RepositoryFiles;

public interface IRepositoryFileStorage
{
    Task SaveAsync(
        Guid repositoryId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken);
}