using EngineerOS.Application.Abstractions.Extraction;

namespace EngineerOS.Application.Features.Repositories.GetCSharpTypes;

public sealed record GetRepositoryCSharpTypesResponse(
    Guid RepositoryId,
    IReadOnlyList<CSharpType> Types);