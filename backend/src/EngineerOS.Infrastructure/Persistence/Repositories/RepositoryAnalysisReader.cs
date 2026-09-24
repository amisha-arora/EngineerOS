using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class RepositoryAnalysisReader
    : IRepositoryAnalysisReader
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAnalysisReader(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<RepositoryFile>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.RepositoryFiles
            .Where(file => file.RepositoryId == repositoryId)
            .OrderBy(file => file.Path)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CodeClass>> GetClassesAsync(
        IReadOnlyList<Guid> repositoryFileIds,
        CancellationToken cancellationToken)
    {
        if (repositoryFileIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.CodeClasses
            .Where(codeClass =>
                repositoryFileIds.Contains(
                    codeClass.RepositoryFileId))
            .OrderBy(codeClass => codeClass.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CodeMethod>> GetMethodsAsync(
        IReadOnlyList<Guid> codeClassIds,
        CancellationToken cancellationToken)
    {
        if (codeClassIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.CodeMethods
            .Where(method =>
                codeClassIds.Contains(
                    method.CodeClassId))
            .OrderBy(method => method.LineNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CodeDependency>> GetDependenciesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.CodeDependencies
            .Where(dependency =>
                dependency.RepositoryId == repositoryId)
            .OrderBy(dependency => dependency.SourceTypeName)
            .ThenBy(dependency => dependency.LineNumber)
            .ToListAsync(cancellationToken);
    }
}