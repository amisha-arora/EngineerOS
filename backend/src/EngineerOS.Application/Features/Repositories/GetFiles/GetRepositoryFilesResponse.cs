namespace EngineerOS.Application.Features.Repositories.GetFiles;

public sealed record GetRepositoryFilesResponse(
    Guid RepositoryId,
    IReadOnlyList<string> Files);