
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class RepositoryAnalysisWriter
    : IRepositoryAnalysisWriter
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAnalysisWriter(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ReplaceMetadataAsync(
        Guid repositoryId,
        IReadOnlyList<RepositoryFile> files,
        IReadOnlyList<CodeClass> codeClasses,
        IReadOnlyList<CodeMethod> codeMethods,
        IReadOnlyList<CodeDependency> codeDependencies,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        // Delete old analysis data first.
        await _dbContext.CodeDependencies
            .Where(dependency =>
                dependency.RepositoryId == repositoryId)
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.CodeMethodParameters
            .Where(parameter =>
                codeMethods
                    .Select(method => method.Id)
                    .Contains(parameter.CodeMethodId))
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.CodeMethods
            .Where(method =>
                codeClasses
                    .Select(codeClass => codeClass.Id)
                    .Contains(method.CodeClassId))
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.CodeClasses
            .Where(codeClass =>
                codeClasses
                    .Select(codeClass => codeClass.Id)
                    .Contains(codeClass.Id))
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.RepositoryFiles
            .Where(file => file.RepositoryId == repositoryId)
            .ExecuteDeleteAsync(cancellationToken);

        // Store repository files.
        await _dbContext.RepositoryFiles.AddRangeAsync(
            files,
            cancellationToken);

        // Store classes.
        await _dbContext.CodeClasses.AddRangeAsync(
            codeClasses,
            cancellationToken);

        // Store methods.
        await _dbContext.CodeMethods.AddRangeAsync(
            codeMethods,
            cancellationToken);

        // Store dependencies.
        await _dbContext.CodeDependencies.AddRangeAsync(
            codeDependencies,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);
    }
}

