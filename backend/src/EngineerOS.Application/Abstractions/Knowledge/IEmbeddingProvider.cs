namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IEmbeddingProvider
{
    Task<EmbeddingResult> GenerateDocumentEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task<EmbeddingResult> GenerateQueryEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}