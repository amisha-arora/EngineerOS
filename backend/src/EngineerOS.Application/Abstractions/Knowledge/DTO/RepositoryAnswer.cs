namespace EngineerOS.Application.Knowledge.DTOs;

public sealed record RepositoryAnswer(
    Guid RepositoryId,
    string Question,
    string Answer,
    IReadOnlyList<string> Sources,
    IReadOnlyList<string> RelevantClasses,
    IReadOnlyList<string> RelevantMethods,
    IReadOnlyList<string> Documentation,
    IReadOnlyList<RepositorySourceCitation> Citations);