//You do not store the temporary chunk result objects directly in the database

namespace EngineerOS.Application.Abstractions.Knowledge;
public sealed record DocumentChunkResult(
    int ChunkIndex,
    string Content,
    string? Section,
    string? Metadata);