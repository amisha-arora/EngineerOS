using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Application.Abstractions.Persistence;

public interface IRepositoryAnalysisWriter
{
    Task ReplaceMetadataAsync(
        Guid repositoryId,
        IReadOnlyList<RepositoryFile> files,
        IReadOnlyList<CSharpType> types,
        IReadOnlyList<CSharpMethod> methods,
        IReadOnlyList<CSharpDependency> dependencies,
        CancellationToken cancellationToken);
}