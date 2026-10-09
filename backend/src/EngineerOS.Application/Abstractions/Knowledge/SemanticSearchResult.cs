namespace EngineerOS.Application.Abstractions.Knowledge;

public sealed record SemanticSearchResult(
    Guid RepositoryId,
    Guid RepositoryFileId,
    string FileName,
    string FilePath,
    Guid ChunkId,
    string ChunkType,
    string? SymbolName,
    string? SymbolType,
    int? StartLine,
    int? EndLine,
    double Score,
    string Content);