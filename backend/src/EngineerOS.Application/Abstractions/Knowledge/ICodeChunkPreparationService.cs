namespace EngineerOS.Application.Abstractions.Knowledge;

public interface ICodeChunkPreparationService
{
    Task PrepareAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default);
}