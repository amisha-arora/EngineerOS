namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryContentExtractionService
{
    //use the ID when working with database entities, or use the direct path when looping through files on disk
    Task<string> ExtractAsync(
        Guid repositoryId,
        Guid repositoryFileId,
        CancellationToken cancellationToken = default);

    Task<string> ExtractAsync(
        Guid repositoryId,
        string relativePath,
        string extension,
        CancellationToken cancellationToken = default);
}