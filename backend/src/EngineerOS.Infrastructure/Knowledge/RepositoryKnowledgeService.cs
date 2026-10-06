using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Entities;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class RepositoryKnowledgeService
    : IRepositoryKnowledgeService
{
    private const int ImportantThreshold = 20;

    private readonly ApplicationDbContext _dbContext;
    private readonly IFileImportanceAnalyzer _importanceAnalyzer;

    public RepositoryKnowledgeService(
        ApplicationDbContext dbContext,
        IFileImportanceAnalyzer importanceAnalyzer)
    {
        _dbContext = dbContext;
        _importanceAnalyzer = importanceAnalyzer;
    }

    public async Task BuildFileKnowledgeAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        var files = await _dbContext
            .Set<RepositoryFile>()
            .Where(x => x.RepositoryId == repositoryId)
            .ToListAsync(cancellationToken);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var score =
                await _importanceAnalyzer.CalculateScoreAsync(
                    repositoryId,
                    file.Id,
                    cancellationToken);

            var isImportant =
                score >= ImportantThreshold;

            var knowledge =
                await _dbContext
                    .Set<FileKnowledge>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.RepositoryFileId == file.Id,
                        cancellationToken);

            if (knowledge is null)
            {
                knowledge = new FileKnowledge(
                    repositoryId,
                    file.Id);

                _dbContext
                    .Set<FileKnowledge>()
                    .Add(knowledge);
            }

            knowledge.SetImportance(
                score,
                isImportant);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}