using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Abstractions.Persistence;

public interface IRepositoryRepository
{
    Task AddAsync(
        Repository repository,
        CancellationToken cancellationToken);

    Task<List<Repository>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Repository?> GetByIdAsync(
        Guid repositoryId,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Repository repository,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}