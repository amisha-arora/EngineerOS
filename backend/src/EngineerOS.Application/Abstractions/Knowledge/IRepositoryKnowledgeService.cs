namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryKnowledgeService
{
    Task BuildFileKnowledgeAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default);
}