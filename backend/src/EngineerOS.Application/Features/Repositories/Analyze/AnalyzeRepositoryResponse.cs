namespace EngineerOS.Application.Features.Repositories.Analyze;

public sealed record AnalyzeRepositoryResponse(
    Guid AnalysisId,
    string Status,
    int FileCount,
    int TypeCount,
    int MethodCount,
    int DependencyCount);