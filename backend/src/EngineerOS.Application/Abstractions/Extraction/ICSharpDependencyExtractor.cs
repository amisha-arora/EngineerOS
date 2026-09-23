namespace EngineerOS.Application.Abstractions.Extraction;

public interface ICSharpDependencyExtractor
{
    Task<IReadOnlyList<CSharpDependency>> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}