using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EngineerOS.Infrastructure.Persistence;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class FileImportanceAnalyzer
    : IFileImportanceAnalyzer
{
    private const int ImportantThreshold = 20;

    private readonly ApplicationDbContext _dbContext;

    public FileImportanceAnalyzer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CalculateScoreAsync(
        Guid repositoryId,
        Guid repositoryFileId,
        CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Set<RepositoryFile>()
            .FirstOrDefaultAsync(
                x => x.Id == repositoryFileId,
                cancellationToken);

        if (file is null)
        {
            throw new InvalidOperationException(
                $"Repository file '{repositoryFileId}' was not found.");
        }

        var score = 0;

        // --------------------------------
        // 1. Entry point
        // --------------------------------

        if (string.Equals(
                file.Name,
                "Program.cs",
                StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
        }

        // --------------------------------
        // 2. Controller / API file
        // --------------------------------

        if (file.Name.EndsWith(
                "Controller.cs",
                StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
        }

        // --------------------------------
        // 3. Classes
        // --------------------------------

        var classCount = await _dbContext
            .Set<CodeClass>()
            .CountAsync(
                x => x.RepositoryFileId == repositoryFileId,
                cancellationToken);

        if (classCount > 0)
        {
            score += 5;
        }

        // --------------------------------
        // 4. Methods
        // --------------------------------

        var methodCount = await _dbContext
            .Set<CodeMethod>()
            .CountAsync(
                x => x.CodeClassId != null &&
                     _dbContext.Set<CodeClass>()
                         .Any(c =>
                             c.Id == x.CodeClassId &&
                             c.RepositoryFileId == repositoryFileId),
                cancellationToken);

        if (methodCount > 0)
        {
            score += 5;
        }

        // --------------------------------
        // 5. Dependents
        // --------------------------------

        var dependentCount = await _dbContext
            .Set<CodeDependency>()
            .CountAsync(
                x => x.TargetCodeClassId != null &&
                     _dbContext.Set<CodeClass>()
                         .Any(c =>
                             c.Id == x.TargetCodeClassId &&
                             c.RepositoryFileId == repositoryFileId),
                cancellationToken);

        score += dependentCount * 5;

        // --------------------------------
        // 6. Dependencies
        // --------------------------------

        var dependencyCount = await _dbContext
            .Set<CodeDependency>()
            .CountAsync(
                x => x.SourceCodeClassId != null &&
                     _dbContext.Set<CodeClass>()
                         .Any(c =>
                             c.Id == x.SourceCodeClassId &&
                             c.RepositoryFileId == repositoryFileId),
                cancellationToken);

        score += dependencyCount * 2;

        return score;
    }
}