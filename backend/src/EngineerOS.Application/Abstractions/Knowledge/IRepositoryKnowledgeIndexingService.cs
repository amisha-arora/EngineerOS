namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryKnowledgeIndexingService
{
    Task<RepositoryIndexingResult> IndexAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default);
}