using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using ApplicationRepositoryFile =
    EngineerOS.Application.Abstractions.Storage.RepositoryFile;
using DomainRepositoryFile =
    EngineerOS.Domain.Entities.RepositoryFile;
using DomainCodeClass =
    EngineerOS.Domain.Entities.CodeClass;
using DomainCodeMethod =
    EngineerOS.Domain.Entities.CodeMethod;
using DomainCodeMethodParameter =
    EngineerOS.Domain.Entities.CodeMethodParameter;
using DomainCodeDependency =
    EngineerOS.Domain.Entities.CodeDependency;

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
        IReadOnlyList<ApplicationRepositoryFile> files,
        IReadOnlyList<CSharpType> types,
        IReadOnlyList<CSharpMethod> methods,
        IReadOnlyList<CSharpDependency> dependencies,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        await _dbContext.CodeDependencies
            .Where(dependency =>
                dependency.RepositoryId == repositoryId)
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.RepositoryFiles
            .Where(file => file.RepositoryId == repositoryId)
            .ExecuteDeleteAsync(cancellationToken);

        var storedFiles = files
            .Select(file => new DomainRepositoryFile(
                repositoryId,
                file.Name,
                file.Path,
                file.Extension,
                file.Type,
                file.Size,
                file.ParentDirectory))
            .ToList();

        await _dbContext.RepositoryFiles.AddRangeAsync(
            storedFiles,
            cancellationToken);

        var filesByPath = storedFiles.ToDictionary(
            file => file.Path,
            StringComparer.Ordinal);

        var storedClasses = new List<DomainCodeClass>();

        foreach (var type in types)
        {
            if (!filesByPath.TryGetValue(
                    type.File,
                    out var storedFile))
            {
                continue;
            }

            storedClasses.Add(new DomainCodeClass(
                storedFile.Id,
                type.Name,
                type.Namespace,
                type.Kind,
                type.Visibility,
                type.Modifier));
        }

        await _dbContext.CodeClasses.AddRangeAsync(
            storedClasses,
            cancellationToken);

        var classesByFileAndName = storedClasses
            .GroupBy(codeClass =>
                $"{codeClass.RepositoryFileId}:{codeClass.Name}")
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.Ordinal);

        var storedMethods = new List<DomainCodeMethod>();

        foreach (var method in methods)
        {
            if (!filesByPath.TryGetValue(
                    method.File,
                    out var storedFile))
            {
                continue;
            }

            var classKey =
                $"{storedFile.Id}:{method.ContainingType}";

            if (!classesByFileAndName.TryGetValue(
                    classKey,
                    out var storedClass))
            {
                continue;
            }

            storedMethods.Add(new DomainCodeMethod(
                storedClass.Id,
                method.Name,
                method.ReturnType,
                method.Visibility,
                method.LineNumber));
        }

        await _dbContext.CodeMethods.AddRangeAsync(
            storedMethods,
            cancellationToken);

        var storedParameters =
            new List<DomainCodeMethodParameter>();

        for (var index = 0; index < methods.Count; index++)
        {
            var method = methods[index];

            if (!filesByPath.TryGetValue(
                    method.File,
                    out var storedFile))
            {
                continue;
            }

            var classKey =
                $"{storedFile.Id}:{method.ContainingType}";

            if (!classesByFileAndName.ContainsKey(classKey))
            {
                continue;
            }

            var storedMethod = storedMethods
                .FirstOrDefault(candidate =>
                    candidate.Name == method.Name &&
                    candidate.LineNumber == method.LineNumber);

            if (storedMethod is null)
            {
                continue;
            }

            for (var parameterIndex = 0;
                 parameterIndex < method.Parameters.Count;
                 parameterIndex++)
            {
                var parameter = method.Parameters[parameterIndex];

                storedParameters.Add(
                    new DomainCodeMethodParameter(
                        storedMethod.Id,
                        parameter.Name,
                        parameter.Type,
                        parameterIndex));
            }
        }

        await _dbContext.CodeMethodParameters.AddRangeAsync(
            storedParameters,
            cancellationToken);

        var storedDependencies =
            new List<DomainCodeDependency>();

        foreach (var dependency in dependencies)
        {
            DomainCodeClass? sourceClass = null;

            if (filesByPath.TryGetValue(
                    dependency.File,
                    out var storedFile))
            {
                classesByFileAndName.TryGetValue(
                    $"{storedFile.Id}:{dependency.SourceType}",
                    out sourceClass);
            }

            var targetClass = storedClasses.FirstOrDefault(
                codeClass =>
                    codeClass.Name == dependency.TargetType);

            storedDependencies.Add(new DomainCodeDependency(
                repositoryId,
                sourceClass?.Id,
                dependency.SourceType,
                targetClass?.Id,
                dependency.TargetType,
                dependency.Relationship,
                dependency.LineNumber));
        }

        await _dbContext.CodeDependencies.AddRangeAsync(
            storedDependencies,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}