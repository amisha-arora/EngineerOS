namespace EngineerOS.Domain.Entities;

public sealed class DocumentChunk
{
    public Guid Id { get; private set; }

    public Guid DocumentId { get; private set; }

    public int ChunkIndex { get; private set; }

    public string Content { get; private set; }

    public string? Section { get; private set; }

    public string? Metadata { get; private set; }

    private DocumentChunk()
    {
        Content = null!;
    }

    public DocumentChunk(
        Guid documentId,
        int chunkIndex,
        string content,
        string? section,
        string? metadata)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        ChunkIndex = chunkIndex;
        Content = content;
        Section = section;
        Metadata = metadata;
    }
}