using System.Data;
using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class SemanticSearchService : ISemanticSearchService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IRepositoryContentExtractionService _contentExtractionService;

    public SemanticSearchService(
        ApplicationDbContext dbContext,
        IEmbeddingProvider embeddingProvider,
        IRepositoryContentExtractionService contentExtractionService)
    {
        _dbContext = dbContext;
        _embeddingProvider = embeddingProvider;
        _contentExtractionService = contentExtractionService;
    }

    public async Task<IReadOnlyList<SemanticSearchResult>> SearchAsync(
        Guid repositoryId,
        string query,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        if (repositoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Repository id is required.",
                nameof(repositoryId));
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Search query cannot be empty.",
                nameof(query));
        }

        if (topK <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topK),
                "Top-K must be greater than zero.");
        }

        // Search text must be embedded as a QUERY,
        // not as a document.
        var queryEmbedding =
            await _embeddingProvider.GenerateQueryEmbeddingAsync(
                query,
                cancellationToken);

        var queryVector =
            new Vector(queryEmbedding.Vector.ToArray());

        var matches = await FindNearestEmbeddingsAsync(
            repositoryId,
            queryVector,
            topK,
            cancellationToken);

        var results = new List<SemanticSearchResult>();

        foreach (var match in matches)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (match.CodeChunkId is not null)
            {
                var result = await BuildCodeResultAsync(
                    repositoryId,
                    match,
                    cancellationToken);

                if (result is not null)
                {
                    results.Add(result);
                }

                continue;
            }

            if (match.DocumentChunkId is not null)
            {
                var result = await BuildDocumentResultAsync(
                    repositoryId,
                    match,
                    cancellationToken);

                if (result is not null)
                {
                    results.Add(result);
                }
            }
        }

        return results;
    }

    private async Task<List<VectorMatch>> FindNearestEmbeddingsAsync(
        Guid repositoryId,
        Vector queryVector,
        int topK,
        CancellationToken cancellationToken)
    {
        var connection =
            (NpgsqlConnection)_dbContext.Database.GetDbConnection();

        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    e."Id",
                    e."DocumentChunkId",
                    e."CodeChunkId",
                    1 - (e."Vector" <=> @queryVector) AS "Score"
                FROM "Embeddings" e
                LEFT JOIN "CodeChunks" cc
                    ON e."CodeChunkId" = cc."Id"
                LEFT JOIN "DocumentChunks" dc
                    ON e."DocumentChunkId" = dc."Id"
                LEFT JOIN "Documents" d
                    ON dc."DocumentId" = d."Id"
                WHERE
                    (
                        cc."RepositoryId" = @repositoryId
                        OR
                        d."RepositoryId" = @repositoryId
                    )
                ORDER BY e."Vector" <=> @queryVector
                LIMIT @topK;
                """;

            command.Parameters.AddWithValue(
                "queryVector",
                queryVector);

            command.Parameters.AddWithValue(
                "repositoryId",
                repositoryId);

            command.Parameters.AddWithValue(
                "topK",
                topK);

            var matches = new List<VectorMatch>();

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                matches.Add(
                    new VectorMatch(
                        reader.GetGuid(0),
                        reader.IsDBNull(1)
                            ? null
                            : reader.GetGuid(1),
                        reader.IsDBNull(2)
                            ? null
                            : reader.GetGuid(2),
                        reader.GetDouble(3)));
            }

            return matches;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private async Task<SemanticSearchResult?> BuildCodeResultAsync(
        Guid repositoryId,
        VectorMatch match,
        CancellationToken cancellationToken)
    {
        var chunk = await _dbContext.CodeChunks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == match.CodeChunkId,
                cancellationToken);

        if (chunk is null)
        {
            return null;
        }

        var file = await _dbContext.RepositoryFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == chunk.RepositoryFileId &&
                    x.RepositoryId == repositoryId,
                cancellationToken);

        if (file is null)
        {
            return null;
        }

        var fileContent =
            await _contentExtractionService.ExtractAsync(
                file.RepositoryId,
                file.Path,
                file.Extension,
                cancellationToken);

        var content = ExtractLines(
            fileContent,
            chunk.StartLine,
            chunk.EndLine);

        return new SemanticSearchResult(
            repositoryId,
            file.Id,
            file.Name,
            file.Path,
            chunk.Id,
            "Code",
            chunk.SymbolName,
            chunk.SymbolType,
            chunk.StartLine,
            chunk.EndLine,
            match.Score,
            content);
    }

    private async Task<SemanticSearchResult?> BuildDocumentResultAsync(
        Guid repositoryId,
        VectorMatch match,
        CancellationToken cancellationToken)
    {
        var chunk = await _dbContext.DocumentChunks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == match.DocumentChunkId,
                cancellationToken);

        if (chunk is null)
        {
            return null;
        }

        var document = await _dbContext.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == chunk.DocumentId &&
                    x.RepositoryId == repositoryId,
                cancellationToken);

        if (document is null)
        {
            return null;
        }

        var file = await _dbContext.RepositoryFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.RepositoryId == repositoryId &&
                    x.Path == document.RelativePath,
                cancellationToken);

        if (file is null)
        {
            return null;
        }

        return new SemanticSearchResult(
            repositoryId,
            file.Id,
            file.Name,
            file.Path,
            chunk.Id,
            "Document",
            null,
            null,
            null,
            null,
            match.Score,
            chunk.Content);
    }

    private static string ExtractLines(
        string content,
        int startLine,
        int endLine)
    {
        var lines = content
            .Replace("\r\n", "\n")
            .Split('\n');

        var start = Math.Max(
            startLine - 1,
            0);

        if (start >= lines.Length)
        {
            return string.Empty;
        }

        var end = Math.Min(
            endLine,
            lines.Length);

        return string.Join(
            Environment.NewLine,
            lines[start..end]);
    }

    private sealed record VectorMatch(
        Guid EmbeddingId,
        Guid? DocumentChunkId,
        Guid? CodeChunkId,
        double Score);
}