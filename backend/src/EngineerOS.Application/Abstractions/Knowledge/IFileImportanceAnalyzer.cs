public interface IFileImportanceAnalyzer
{
    Task<int> CalculateScoreAsync(
        Guid repositoryId,
        Guid repositoryFileId,
        CancellationToken cancellationToken = default);
}