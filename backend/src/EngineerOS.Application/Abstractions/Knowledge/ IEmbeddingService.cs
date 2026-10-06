namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IEmbeddingService
{
    Task GenerateForDocumentChunkAsync(
        Guid documentChunkId,
        CancellationToken cancellationToken = default);

    Task GenerateForCodeChunkAsync(
        Guid codeChunkId,
        CancellationToken cancellationToken = default);
}