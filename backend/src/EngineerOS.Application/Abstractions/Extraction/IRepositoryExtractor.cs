namespace EngineerOS.Application.Abstractions.Extraction;

public interface IRepositoryExtractor
{
    Task<string> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}
