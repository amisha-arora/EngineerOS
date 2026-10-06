namespace EngineerOS.Domain.Entities;

public sealed class Document
{
    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public string Name { get; private set; }

    public string RelativePath { get; private set; }

    public string DocumentType { get; private set; }

    public long FileSize { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Document()
    {
        Name = null!;
        RelativePath = null!;
        DocumentType = null!;
    }

    public Document(
        Guid repositoryId,
        string name,
        string relativePath,
        string documentType,
        long fileSize)
    {
        Id = Guid.NewGuid();
        RepositoryId = repositoryId;
        Name = name;
        RelativePath = relativePath;
        DocumentType = documentType;
        FileSize = fileSize;
        CreatedAt = DateTime.UtcNow;
    }
}