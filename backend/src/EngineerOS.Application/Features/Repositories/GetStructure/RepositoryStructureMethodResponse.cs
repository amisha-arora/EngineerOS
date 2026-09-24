namespace EngineerOS.Application.Features.Repositories.GetStructure;

public sealed record RepositoryStructureMethodResponse(
    Guid Id,
    string Name,
    string ReturnType,
    string AccessModifier,
    int LineNumber);