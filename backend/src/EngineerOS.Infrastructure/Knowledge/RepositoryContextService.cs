using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Knowledge.DTOs;
using EngineerOS.Domain.Entities;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class RepositoryContextService
    : IRepositoryContextService
{

    private const int MaxContextFiles = 20;

    private readonly ApplicationDbContext _dbContext;
    private readonly ISemanticSearchService _semanticSearchService;

    public RepositoryContextService(
        ApplicationDbContext dbContext,
        ISemanticSearchService semanticSearchService)
    {
        _dbContext = dbContext;
        _semanticSearchService = semanticSearchService;
    }

    public async Task<RepositoryContext> BuildAsync(
        Guid repositoryId,
        string query,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        // ---------------------------------------------------------
        // VALIDATION
        // ---------------------------------------------------------

        if (repositoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Repository id is required.",
                nameof(repositoryId));
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Query cannot be empty.",
                nameof(query));
        }

        if (topK <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topK),
                "Top-K must be greater than zero.");
        }


        // ---------------------------------------------------------
        // STEP 1:
        // SEMANTIC SEARCH
        //
        // Day 9 semantic search gives us the initial set of
        // relevant chunks/files.
        // ---------------------------------------------------------

        var semanticResults =
            await _semanticSearchService.SearchAsync(
                repositoryId,
                query,
                topK,
                cancellationToken);

        if (semanticResults.Count == 0)
        {
            return new RepositoryContext(
                repositoryId,
                query,
                Array.Empty<RepositoryContextFile>());
        }


        // ---------------------------------------------------------
        // STEP 2:
        // SEMANTIC SEED FILES
        //
        // Multiple chunks may belong to the same file.
        // Keep the file IDs separately so later we know which
        // files came directly from semantic search.
        // ---------------------------------------------------------

        var semanticFileIds = semanticResults
            .Select(x => x.RepositoryFileId)
            .Distinct()
            .ToHashSet();

        var candidateFileIds = semanticFileIds.ToList();


        // ---------------------------------------------------------
        // STEP 3:
        // FIND CLASSES INSIDE SEMANTIC SEED FILES
        //
        // These classes become our starting nodes in the
        // Code Knowledge Graph.
        // ---------------------------------------------------------

        var seedClasses = await _dbContext
            .Set<CodeClass>()
            .AsNoTracking()
            .Where(x =>
                candidateFileIds.Contains(x.RepositoryFileId))
            .ToListAsync(cancellationToken);

        var seedClassIds = seedClasses
            .Select(x => x.Id)
            .ToList();


        // Tracks files discovered specifically through
        // CodeDependency relationships.
        var graphFileIds = new HashSet<Guid>();


        // ---------------------------------------------------------
        // STEP 4:
        // ONE-HOP CODE KNOWLEDGE GRAPH EXPANSION
        //
        // For each semantic seed class, find:
        //
        // SeedClass ---> TargetClass
        //
        // AND
        //
        // SourceClass ---> SeedClass
        //
        // This gives us dependencies AND dependents.
        // ---------------------------------------------------------

        if (seedClassIds.Count > 0)
        {
            var relatedClassIds = await _dbContext
                .Set<CodeDependency>()
                .AsNoTracking()
                .Where(x =>
                    x.RepositoryId == repositoryId &&
                    (
                        (
                            x.SourceCodeClassId.HasValue &&
                            seedClassIds.Contains(
                                x.SourceCodeClassId.Value)
                        )
                        ||
                        (
                            x.TargetCodeClassId.HasValue &&
                            seedClassIds.Contains(
                                x.TargetCodeClassId.Value)
                        )
                    ))
                .Select(x =>
                    x.SourceCodeClassId.HasValue &&
                    seedClassIds.Contains(
                        x.SourceCodeClassId.Value)

                        // Seed is the source.
                        // Return the target.
                        ? x.TargetCodeClassId

                        // Otherwise seed is the target.
                        // Return the source.
                        : x.SourceCodeClassId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);


            // -----------------------------------------------------
            // STEP 5:
            // RELATED CLASSES -> RELATED FILES
            // -----------------------------------------------------

            if (relatedClassIds.Count > 0)
            {
                var relatedFileIds = await _dbContext
                    .Set<CodeClass>()
                    .AsNoTracking()
                    .Where(x =>
                        relatedClassIds.Contains(x.Id))
                    .Select(x => x.RepositoryFileId)
                    .Distinct()
                    .ToListAsync(cancellationToken);


                // Remember which files were discovered through
                // graph expansion rather than semantic search.
                foreach (var relatedFileId in relatedFileIds)
                {
                    if (!semanticFileIds.Contains(relatedFileId))
                    {
                        graphFileIds.Add(relatedFileId);
                    }
                }


                // Final candidate set:
                //
                // semantic files
                // +
                // graph-discovered files
                candidateFileIds = candidateFileIds
                    .Concat(relatedFileIds)
                    .Distinct()
                    .ToList();
            }
        }


        // ---------------------------------------------------------
        // STEP 6:
        // LOAD ONLY CANDIDATE FILES
        //
        // Important:
        // We do NOT load the entire repository.
        // ---------------------------------------------------------

        var files = await _dbContext.RepositoryFiles
            .AsNoTracking()
            .Where(x =>
                x.RepositoryId == repositoryId &&
                candidateFileIds.Contains(x.Id))
            .ToListAsync(cancellationToken);


        var contextFiles =
            new List<RepositoryContextFile>();


        // ---------------------------------------------------------
        // STEP 7:
        // ENRICH EACH CANDIDATE FILE
        // ---------------------------------------------------------

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();


            // -----------------------------------------------------
            // FILE IMPORTANCE
            // -----------------------------------------------------

            var fileKnowledge = await _dbContext
                .Set<FileKnowledge>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.RepositoryId == repositoryId &&
                        x.RepositoryFileId == file.Id,
                    cancellationToken);


            // -----------------------------------------------------
            // CLASSES
            // -----------------------------------------------------

            var classes = await _dbContext
                .Set<CodeClass>()
                .AsNoTracking()
                .Where(x =>
                    x.RepositoryFileId == file.Id)
                .ToListAsync(cancellationToken);

            var classIds = classes
                .Select(x => x.Id)
                .ToList();


            // -----------------------------------------------------
            // METHODS
            // -----------------------------------------------------

            var methods = await _dbContext
                .Set<CodeMethod>()
                .AsNoTracking()
                .Where(x =>
                    classIds.Contains(x.CodeClassId))
                .ToListAsync(cancellationToken);


            // -----------------------------------------------------
            // OUTGOING DEPENDENCIES
            //
            // CurrentFile ---> OtherClass
            // -----------------------------------------------------

            var outgoingDependencies = await _dbContext
                .Set<CodeDependency>()
                .AsNoTracking()
                .Where(x =>
                    x.RepositoryId == repositoryId &&
                    x.SourceCodeClassId.HasValue &&
                    classIds.Contains(
                        x.SourceCodeClassId.Value))
                .Select(x => x.TargetTypeName)
                .Distinct()
                .ToListAsync(cancellationToken);


            // -----------------------------------------------------
            // INCOMING DEPENDENCIES / DEPENDENTS
            //
            // OtherClass ---> CurrentFile
            // -----------------------------------------------------

            var incomingDependencies = await _dbContext
                .Set<CodeDependency>()
                .AsNoTracking()
                .Where(x =>
                    x.RepositoryId == repositoryId &&
                    x.TargetCodeClassId.HasValue &&
                    classIds.Contains(
                        x.TargetCodeClassId.Value))
                .Select(x => x.SourceTypeName)
                .Distinct()
                .ToListAsync(cancellationToken);


            // -----------------------------------------------------
            // SEMANTIC CHUNKS
            //
            // Graph-only files may have zero semantic chunks.
            // That is expected.
            // -----------------------------------------------------

            var relevantChunks = semanticResults
                .Where(x =>
                    x.RepositoryFileId == file.Id)
                .OrderByDescending(x => x.Score)
                .ToList();


            // -----------------------------------------------------
            // SEMANTIC SCORE
            //
            // Use the best matching chunk from the file.
            // -----------------------------------------------------

            var semanticScore = relevantChunks.Count > 0
                ? relevantChunks.Max(x => x.Score)
                : 0.0;


            // -----------------------------------------------------
            // GRAPH SCORE
            //
            // 1.0 = discovered through one-hop graph expansion.
            // 0.0 = not discovered through graph expansion.
            // -----------------------------------------------------

            var graphScore =
                graphFileIds.Contains(file.Id)
                    ? 1.0
                    : 0.0;


            // -----------------------------------------------------
            // FILE IMPORTANCE SCORE
            // -----------------------------------------------------

            var importanceScore =
                fileKnowledge?.ImportanceScore ?? 0;


            // File importance currently uses an integer score.
            // Normalize it to approximately 0-1 before combining
            // it with semantic and graph scores.
            var normalizedImportance =
                Math.Clamp(
                    importanceScore / 100.0,
                    0.0,
                    1.0);


            // -----------------------------------------------------
            // FINAL REPOSITORY-AWARE RELEVANCE SCORE
            //
            // Initial heuristic:
            //
            // Semantic similarity = 70%
            // Graph relationship  = 20%
            // File importance     = 10%
            //
            // These weights can be tuned later.
            // -----------------------------------------------------

            var relevanceScore =
                (semanticScore * 0.70)
                + (graphScore * 0.20)
                + (normalizedImportance * 0.10);


            // -----------------------------------------------------
            // BUILD CONTEXT FILE
            // -----------------------------------------------------

            var contextFile =
                new RepositoryContextFile(
                    file.Id,
                    file.Name,
                    file.Path,
                    file.Type,

                    importanceScore,

                    semanticScore,
                    graphScore,
                    relevanceScore,

                    classes
                        .Select(x => x.Name)
                        .Distinct()
                        .ToList(),

                    methods
                        .Select(x => x.Name)
                        .Distinct()
                        .ToList(),

                    outgoingDependencies,

                    incomingDependencies,

                    relevantChunks);

            contextFiles.Add(contextFile);
        }


        // ---------------------------------------------------------
        // STEP 8:
        // RANK FINAL FILES
        //
        // Repository-aware relevance is now the primary signal.
        // ---------------------------------------------------------

        var orderedFiles = contextFiles
            .OrderByDescending(x => x.RelevanceScore)
            .ThenByDescending(x => x.ImportanceScore)
            .Take(MaxContextFiles)
            .ToList();


        // ---------------------------------------------------------
        // STEP 9:
        // RETURN FOCUSED REPOSITORY CONTEXT
        // ---------------------------------------------------------

        return new RepositoryContext(
            repositoryId,
            query,
            orderedFiles);
    }
}