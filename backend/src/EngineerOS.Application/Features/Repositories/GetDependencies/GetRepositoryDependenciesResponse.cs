namespace EngineerOS.Application.Features.Repositories.GetDependencies;

public sealed record GetRepositoryDependenciesResponse(
    Guid RepositoryId,
    IReadOnlyList<RepositoryDependencyResponse> Dependencies);