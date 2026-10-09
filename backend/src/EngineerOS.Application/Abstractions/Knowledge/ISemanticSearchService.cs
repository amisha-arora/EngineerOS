namespace EngineerOS.Application.Abstractions.Knowledge;

public interface ISemanticSearchService
{
    Task<IReadOnlyList<SemanticSearchResult>> SearchAsync(
        Guid repositoryId,
        string query,
        // The maximum number of results to return. Default is 10. similarity = 1 - cosineDistance
        int topK = 10,
        CancellationToken cancellationToken = default);
}