namespace EngineerOS.Application.Knowledge.DTOs;

public sealed record RepositorySourceCitation(
    Guid RepositoryFileId,
    string FileName,
    string FilePath,
    Guid ChunkId,
    string ChunkType,
    string? SymbolName,
    string? SymbolType,
    int? StartLine,
    int? EndLine,
    double RelevanceScore);