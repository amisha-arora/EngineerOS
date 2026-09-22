namespace EngineerOS.Application.Abstractions.Extraction;

public interface ICSharpTypeExtractor
{
    Task<IReadOnlyList<CSharpType>> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);
}