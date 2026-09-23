using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class RepositoryAnalysisRepository
    : IRepositoryAnalysisRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAnalysisRepository(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        RepositoryAnalysis analysis,
        CancellationToken cancellationToken)
    {
        await _dbContext.RepositoryAnalyses.AddAsync(
            analysis,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}