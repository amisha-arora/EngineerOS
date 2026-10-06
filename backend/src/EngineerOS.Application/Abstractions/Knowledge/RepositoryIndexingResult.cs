namespace EngineerOS.Application.Abstractions.Knowledge;

public sealed record RepositoryIndexingResult(
    Guid RepositoryId,
    int FilesProcessed,
    int ImportantFiles,
    int DocumentChunks,
    int CodeChunks,
    int EmbeddingsGenerated); 