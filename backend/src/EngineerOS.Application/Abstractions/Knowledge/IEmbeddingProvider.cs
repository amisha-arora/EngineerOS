namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IEmbeddingProvider
{
    Task<EmbeddingResult> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}