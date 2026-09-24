using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Abstractions.Persistence;

public interface IRepositoryAnalysisReader
{
    Task<IReadOnlyList<RepositoryFile>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CodeClass>> GetClassesAsync(
        IReadOnlyList<Guid> repositoryFileIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CodeMethod>> GetMethodsAsync(
        IReadOnlyList<Guid> codeClassIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CodeDependency>> GetDependenciesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}