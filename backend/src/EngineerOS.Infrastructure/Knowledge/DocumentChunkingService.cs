using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Entities;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class DocumentChunkingService
    : IDocumentChunkingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDocumentChunker _chunker;

    public DocumentChunkingService(
        ApplicationDbContext dbContext,
        IDocumentChunker chunker)
    {
        _dbContext = dbContext;
        _chunker = chunker;
    }

    public async Task ChunkDocumentAsync(
        Guid documentId,
        Guid repositoryId,
        string content,
        CancellationToken cancellationToken = default)
    {
        //Before doing any expensive text processing, it verifies that the document actually exists in the database and belongs to the specified repository.
        var documentExists =
            await _dbContext.Set<Document>()
                .AnyAsync(
                    x => x.Id == documentId &&
                         x.RepositoryId == repositoryId,
                    cancellationToken);

        if (!documentExists)
        {
            throw new InvalidOperationException(
                $"Document '{documentId}' was not found.");
        }
        //Calls the pure text chunker you wrote earlier. It passes in the raw string content and receives back an in-memory list of DocumentChunkResult DTOs
        var chunks = _chunker.Chunk(content);

        //If a document is updated and re-chunked, this code cleans up any previously stored chunks for that document first.
        var existingChunks =
            await _dbContext.Set<DocumentChunk>()
                .Where(x => x.DocumentId == documentId)
                .ToListAsync(cancellationToken);

        _dbContext.Set<DocumentChunk>()
            .RemoveRange(existingChunks);

        //This is where the separation pays off. It takes each temporary chunk DTO (from memory) and translates it into a permanent DocumentChunk database entity, tying it to the documentId
        foreach (var chunk in chunks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _dbContext.Set<DocumentChunk>()
                .Add(
                    new DocumentChunk(
                        documentId,
                        chunk.ChunkIndex,
                        chunk.Content,
                        chunk.Section,
                        chunk.Metadata));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}