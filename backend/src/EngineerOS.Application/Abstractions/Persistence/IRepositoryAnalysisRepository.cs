using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Abstractions.Persistence;

public interface IRepositoryAnalysisRepository
{
    Task AddAsync(
        RepositoryAnalysis analysis,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}