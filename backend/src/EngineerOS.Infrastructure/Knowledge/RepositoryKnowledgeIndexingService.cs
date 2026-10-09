using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Abstractions.RepositoryFiles;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EngineerOS.Domain.Entities;


namespace EngineerOS.Infrastructure.Knowledge;

public sealed class RepositoryKnowledgeIndexingService
    : IRepositoryKnowledgeIndexingService
{
    private readonly IRepositoryFileReader _fileReader;
    private readonly IRepositoryKnowledgeService _repositoryKnowledgeService;
    private readonly IDocumentationDiscoveryService _documentationDiscoveryService;
    private readonly IRepositoryContentExtractionService _contentExtractionService;
    private readonly IDocumentChunkingService _documentChunkingService;
    private readonly ICodeChunkPreparationService _codeChunkPreparationService;
    private readonly IFileKnowledgePreparationService _fileKnowledgePreparationService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ApplicationDbContext _dbContext;

    public RepositoryKnowledgeIndexingService(
        IRepositoryFileReader fileReader,
        IRepositoryKnowledgeService repositoryKnowledgeService,
        IDocumentationDiscoveryService documentationDiscoveryService,
        IRepositoryContentExtractionService contentExtractionService,
        IDocumentChunkingService documentChunkingService,
        ICodeChunkPreparationService codeChunkPreparationService,
        IFileKnowledgePreparationService fileKnowledgePreparationService,
        IEmbeddingService embeddingService,
        ApplicationDbContext dbContext)
    {
        _fileReader = fileReader;
        _repositoryKnowledgeService = repositoryKnowledgeService;
        _documentationDiscoveryService = documentationDiscoveryService;
        _contentExtractionService = contentExtractionService;
        _documentChunkingService = documentChunkingService;
        _codeChunkPreparationService = codeChunkPreparationService;
        _fileKnowledgePreparationService = fileKnowledgePreparationService;
        _embeddingService = embeddingService;
        _dbContext = dbContext;
    }

    public async Task<RepositoryIndexingResult> IndexAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {
        // 1. Get repository files
        var files = await _fileReader.GetFilesAsync(
            repositoryId,
            cancellationToken);

        // 2. Build file importance / FileKnowledge
        await _repositoryKnowledgeService.BuildFileKnowledgeAsync(
            repositoryId,
            cancellationToken);

        // 3. Prepare graph-aware file knowledge
        var fileKnowledge =
            await _fileKnowledgePreparationService.PrepareAsync(
                repositoryId,
                cancellationToken);

        // 4. Discover documentation
        var documents =
            await _documentationDiscoveryService.DiscoverAsync(
                repositoryId,
                cancellationToken);

        // 5. Persist, extract, and chunk documentation
        foreach (var documentDto in documents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Find the corresponding repository file.
            var repositoryFile = files.FirstOrDefault(
                file => file.Path == documentDto.Path);

            if (repositoryFile is null)
            {
                continue;
            }

            // Extract documentation content.
            var content =
                await _contentExtractionService.ExtractAsync(
                    repositoryId,
                    repositoryFile.Path,
                    repositoryFile.Extension,
                    cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
            {
                continue;
            }

            // Find the existing Document entity.
            var persistedDocument = await _dbContext
                .Set<Document>()
                .FirstOrDefaultAsync(
                    d => d.RepositoryId == repositoryId &&
                         d.RelativePath == repositoryFile.Path,
                    cancellationToken);

            // Create the Document only if it does not exist.
            if (persistedDocument is null)
            {
                persistedDocument = new Document(
                    repositoryId,
                    repositoryFile.Name,
                    repositoryFile.Path,
                    repositoryFile.Extension,
                    repositoryFile.Size);

                _dbContext.Set<Document>().Add(persistedDocument);

                await _dbContext.SaveChangesAsync(
                    cancellationToken);
            }

            // Use the actual persisted Document.Id.
            await _documentChunkingService.ChunkDocumentAsync(
                persistedDocument.Id,
                repositoryId,
                content,
                cancellationToken);
        }

        // 6. Prepare code chunks
        await _codeChunkPreparationService.PrepareAsync(
            repositoryId,
            cancellationToken);

        // 7. Load generated chunks
        var documentChunkIds =
            await _dbContext.DocumentChunks
                .Where(x =>
                    _dbContext.Documents
                        .Where(d => d.RepositoryId == repositoryId)
                        .Select(d => d.Id)
                        .Contains(x.DocumentId))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

        var codeChunkIds =
            await _dbContext.CodeChunks
                .Where(x => x.RepositoryId == repositoryId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

        // 8. Generate document embeddings
        foreach (var chunkId in documentChunkIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _embeddingService.GenerateForDocumentChunkAsync(
                chunkId,
                cancellationToken);
        }

        // 9. Generate code embeddings
        foreach (var chunkId in codeChunkIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _embeddingService.GenerateForCodeChunkAsync(
                chunkId,
                cancellationToken);
        }

        // 10. Return indexing statistics
        return new RepositoryIndexingResult(
            repositoryId,
            files.Count,
            fileKnowledge.Count(x => x.IsImportant),
            documentChunkIds.Count,
            codeChunkIds.Count,
            documentChunkIds.Count + codeChunkIds.Count);
    }
}