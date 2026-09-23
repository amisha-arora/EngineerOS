using EngineerOS.Application.Abstractions.Extraction;

namespace EngineerOS.Application.Features.Repositories.GetCSharpDependencies;

public sealed record GetRepositoryCSharpDependenciesResponse(
    Guid RepositoryId,
    IReadOnlyList<CSharpDependency> Dependencies);