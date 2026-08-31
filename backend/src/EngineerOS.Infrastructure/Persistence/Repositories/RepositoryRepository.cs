using EngineerOS.Application.Abstractions.Persistence; 
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class RepositoryRepository : IRepositoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Repository repository,
        CancellationToken cancellationToken)
    {
        await _dbContext.Repositories.AddAsync(
            repository,
            cancellationToken);
    }

    public async Task<List<Repository>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Repositories
            .Where(repository => repository.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Repository?> GetByIdAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Repositories
            .FirstOrDefaultAsync(
                repository => repository.Id == repositoryId,
                cancellationToken);
    }

    public Task DeleteAsync(
        Repository repository,
        CancellationToken cancellationToken)
    {
        _dbContext.Repositories.Remove(repository);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
