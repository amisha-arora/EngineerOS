using EngineerOS.Domain.Entities;
namespace EngineerOS.Application.Abstractions.Persistence;

public interface IRepositoryAnalysisWriter
{
    Task ReplaceMetadataAsync(
        Guid repositoryId,
        IReadOnlyList<RepositoryFile> files,
        IReadOnlyList<CodeClass> types,
        IReadOnlyList<CodeMethod> methods,
        IReadOnlyList<CodeDependency> dependencies,
        CancellationToken cancellationToken);
}

