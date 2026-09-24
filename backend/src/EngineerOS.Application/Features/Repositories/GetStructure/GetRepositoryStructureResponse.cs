namespace EngineerOS.Application.Features.Repositories.GetStructure;

public sealed record GetRepositoryStructureResponse(
    Guid RepositoryId,
    IReadOnlyList<RepositoryStructureFileResponse> Files);