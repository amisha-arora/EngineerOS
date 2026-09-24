namespace EngineerOS.Application.Features.Repositories.GetDependencies;

public sealed record RepositoryDependencyResponse(
    Guid Id,
    Guid? SourceCodeClassId,
    string SourceTypeName,
    Guid? TargetCodeClassId,
    string TargetTypeName,
    string DependencyType,
    int LineNumber);