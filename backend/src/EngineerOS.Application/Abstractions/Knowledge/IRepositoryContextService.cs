using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryContextService
{
    Task<RepositoryContext> BuildAsync(
        Guid repositoryId,
        string query,
        int topK = 10,
        CancellationToken cancellationToken = default);
}