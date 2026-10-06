using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Repositories.GetFiles;

public sealed record GetRepositoryFilesResponse(
    Guid RepositoryId,
    IReadOnlyList<RepositoryFile> Files);