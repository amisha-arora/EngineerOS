namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IDocumentChunkingService
{
    Task ChunkDocumentAsync(
        Guid documentId,
        Guid repositoryId,
        string content,
        CancellationToken cancellationToken = default);
}