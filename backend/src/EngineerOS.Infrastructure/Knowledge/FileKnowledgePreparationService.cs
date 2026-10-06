using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class FileKnowledgePreparationService
    : IFileKnowledgePreparationService
{
    private readonly ApplicationDbContext _dbContext;

    public FileKnowledgePreparationService(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<FileKnowledgeContext>> PrepareAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        var files = await _dbContext
            .Set<EngineerOS.Domain.Entities.RepositoryFile>()
            .Where(x => x.RepositoryId == repositoryId)
            .ToListAsync(cancellationToken);

        var result =
            new List<FileKnowledgeContext>();

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var knowledge =
                await _dbContext
                    .Set<EngineerOS.Domain.Entities.FileKnowledge>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.RepositoryFileId == file.Id &&
                            x.RepositoryId == repositoryId,
                        cancellationToken);

            var classes = await _dbContext
                .Set<EngineerOS.Domain.Entities.CodeClass>()
                .Where(x => x.RepositoryFileId == file.Id)
                .Select(x => x.Name)
                .ToListAsync(cancellationToken);

            var classIds = await _dbContext
                .Set<EngineerOS.Domain.Entities.CodeClass>()
                .Where(x => x.RepositoryFileId == file.Id)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var methods = await _dbContext
                .Set<EngineerOS.Domain.Entities.CodeMethod>()
                .Where(x =>
                    classIds.Contains(x.CodeClassId))
                .Select(x => x.Name)
                .ToListAsync(cancellationToken);

            //Finds what other types/classes this file's classes depend on (what it calls or inherits from)
            var dependencies = await _dbContext
                .Set<EngineerOS.Domain.Entities.CodeDependency>()
                .Where(x =>
                    x.SourceCodeClassId != null &&
                    classIds.Contains(
                        x.SourceCodeClassId.Value))
                .Select(x => x.TargetTypeName)
                .ToListAsync(cancellationToken);

            var dependents = await _dbContext
                .Set<EngineerOS.Domain.Entities.CodeDependency>()
                .Where(x =>
                    x.TargetCodeClassId != null &&
                    classIds.Contains(
                        x.TargetCodeClassId.Value))
                .Select(x => x.SourceTypeName)
                .ToListAsync(cancellationToken);

            result.Add(
                new FileKnowledgeContext(
                    file.Id,
                    file.Name,
                    file.Path,
                    file.Type,
                    knowledge?.IsImportant ?? false,
                    knowledge?.ImportanceScore ?? 0,
                    classes,
                    methods,
                    dependencies,
                    dependents));
        }

        return result;
    }
}