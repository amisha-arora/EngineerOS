using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Entities;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class EmbeddingService : IEmbeddingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IRepositoryContentExtractionService _contentExtractionService;

    public EmbeddingService(
        ApplicationDbContext dbContext,
        IEmbeddingProvider embeddingProvider,
        IRepositoryContentExtractionService contentExtractionService)
    {
        _dbContext = dbContext;
        _embeddingProvider = embeddingProvider;
        _contentExtractionService = contentExtractionService;
    }

    public async Task GenerateForDocumentChunkAsync(
        Guid documentChunkId,
        CancellationToken cancellationToken = default)
    {
        var chunk = await _dbContext.DocumentChunks
            .FirstOrDefaultAsync(
                x => x.Id == documentChunkId,
                cancellationToken);

        if (chunk is null)
        {
            throw new InvalidOperationException(
                $"Document chunk '{documentChunkId}' was not found.");
        }

        var document = await _dbContext.Documents
            .FirstOrDefaultAsync(
                x => x.Id == chunk.DocumentId,
                cancellationToken);

        if (document is null)
        {
            throw new InvalidOperationException(
                $"Document for chunk '{documentChunkId}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(chunk.Content))
        {
            throw new InvalidOperationException(
                $"Document chunk '{documentChunkId}' contains no searchable content.");
        }

        var result =
            await GenerateWithRetryAsync(
                chunk.Content,
                cancellationToken);

        var embedding = new Embedding(
            documentChunkId,
            null,
            result.Vector.ToArray(),
            result.Model);

        await ReplaceDocumentEmbeddingAsync(
            documentChunkId,
            embedding,
            cancellationToken);
    }

    public async Task GenerateForCodeChunkAsync(
        Guid codeChunkId,
        CancellationToken cancellationToken = default)
    {
        var chunk = await _dbContext.CodeChunks
            .FirstOrDefaultAsync(
                x => x.Id == codeChunkId,
                cancellationToken);

        if (chunk is null)
        {
            throw new InvalidOperationException(
                $"Code chunk '{codeChunkId}' was not found.");
        }

        var file = await _dbContext.RepositoryFiles
            .FirstOrDefaultAsync(
                x => x.Id == chunk.RepositoryFileId,
                cancellationToken);

        if (file is null)
        {
            throw new InvalidOperationException(
                $"Repository file for code chunk '{codeChunkId}' was not found.");
        }

        // Use the persisted file path instead of trying to rediscover
        // the RepositoryFile using a regenerated Guid.
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

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                $"Code chunk '{codeChunkId}' contains no searchable content.");
        }

        var result =
            await GenerateWithRetryAsync(
                content,
                cancellationToken);

        var embedding = new Embedding(
            null,
            codeChunkId,
            result.Vector.ToArray(),
            result.Model);

        await ReplaceCodeEmbeddingAsync(
            codeChunkId,
            embedding,
            cancellationToken);
    }

    private async Task<EmbeddingResult> GenerateWithRetryAsync(
        string content,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // These are repository chunks being indexed,
                // so Voyage should receive input_type = "document".
                return await _embeddingProvider
                    .GenerateDocumentEmbeddingAsync(
                        content,
                        cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch when (attempt < maxAttempts)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(attempt),
                    cancellationToken);
            }
        }

        throw new InvalidOperationException(
            "Embedding generation failed after all retry attempts.");
    }

    private async Task ReplaceDocumentEmbeddingAsync(
        Guid documentChunkId,
        Embedding embedding,
        CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Embeddings
            .Where(x => x.DocumentChunkId == documentChunkId)
            .ToListAsync(cancellationToken);

        _dbContext.Embeddings.RemoveRange(existing);

        await _dbContext.Embeddings.AddAsync(
            embedding,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task ReplaceCodeEmbeddingAsync(
        Guid codeChunkId,
        Embedding embedding,
        CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Embeddings
            .Where(x => x.CodeChunkId == codeChunkId)
            .ToListAsync(cancellationToken);

        _dbContext.Embeddings.RemoveRange(existing);

        await _dbContext.Embeddings.AddAsync(
            embedding,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
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
}