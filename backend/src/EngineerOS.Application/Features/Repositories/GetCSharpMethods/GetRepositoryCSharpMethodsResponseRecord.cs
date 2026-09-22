using EngineerOS.Application.Abstractions.Extraction;

namespace EngineerOS.Application.Features.Repositories.GetCSharpMethods;

public sealed record GetRepositoryCSharpMethodsResponse(
    Guid RepositoryId,
    IReadOnlyList<CSharpMethod> Methods);
