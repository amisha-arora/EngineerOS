using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Application.Features.Repositories.GetFiles;

public sealed record GetRepositoryFilesResponse(
    Guid RepositoryId,
    IReadOnlyList<RepositoryFile> Files);