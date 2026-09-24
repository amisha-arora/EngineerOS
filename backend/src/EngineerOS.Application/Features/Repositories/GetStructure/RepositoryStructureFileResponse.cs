namespace EngineerOS.Application.Features.Repositories.GetStructure;

public sealed record RepositoryStructureFileResponse(
    Guid Id,
    string Name,
    string Path,
    string Extension,
    string Type,
    long Size,
    string ParentDirectory,
    IReadOnlyList<RepositoryStructureTypeResponse> Types);