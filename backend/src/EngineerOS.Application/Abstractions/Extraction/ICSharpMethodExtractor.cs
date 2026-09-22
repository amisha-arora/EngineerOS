namespace EngineerOS.Application.Abstractions.Extraction;

public interface ICSharpMethodExtractor
{
    Task<IReadOnlyList<CSharpMethod>> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}