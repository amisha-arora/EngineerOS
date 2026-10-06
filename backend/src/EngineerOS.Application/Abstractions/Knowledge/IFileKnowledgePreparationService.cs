namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IFileKnowledgePreparationService
{
    Task<IReadOnlyList<FileKnowledgeContext>> PrepareAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default);
}