namespace EngineerOS.Application.Abstractions.Knowledge;

public sealed record FileKnowledgeContext(
    Guid RepositoryFileId,
    string FileName,
    string FilePath,
    string FileType,
    bool IsImportant,
    int ImportanceScore,
    IReadOnlyList<string> Classes,
    IReadOnlyList<string> Methods,
    IReadOnlyList<string> Dependencies,
    IReadOnlyList<string> Dependents);