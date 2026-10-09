using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Application.Knowledge.DTOs;

public sealed record RepositoryContextFile(
    Guid RepositoryFileId,
    string FileName,
    string FilePath,
    string FileType,

    int ImportanceScore,

    double SemanticScore,
    double GraphScore,
    double RelevanceScore,

    IReadOnlyList<string> Classes,
    IReadOnlyList<string> Methods,
    IReadOnlyList<string> Dependencies,
    IReadOnlyList<string> Dependents,

    IReadOnlyList<SemanticSearchResult> RelevantChunks);