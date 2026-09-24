namespace EngineerOS.Application.Features.Repositories.GetStructure;

public sealed record RepositoryStructureTypeResponse(
    Guid Id,
    string Name,
    string Namespace,
    string Kind,
    string AccessModifier,
    string? Modifier,
    IReadOnlyList<RepositoryStructureMethodResponse> Methods);