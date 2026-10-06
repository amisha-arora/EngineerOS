namespace EngineerOS.Domain.Entities;

public sealed class FileKnowledge
{
    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public Guid RepositoryFileId { get; private set; }

    public string? Purpose { get; private set; }

    public string? Summary { get; private set; }

    public string? ArchitecturalRole { get; private set; }

    public bool IsImportant { get; private set; }

    public int ImportanceScore { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }


    // EF Core constructor
    private FileKnowledge()
    {
    }


    // Domain constructor
    public FileKnowledge(
        Guid repositoryId,
        Guid repositoryFileId)
    {
        Id = Guid.NewGuid();

        RepositoryId = repositoryId;
        RepositoryFileId = repositoryFileId;

        IsImportant = false;
        ImportanceScore = 0;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }


    // Updates importance information
    public void SetImportance(
        int importanceScore,
        bool isImportant)
    {
        ImportanceScore = importanceScore;
        IsImportant = isImportant;
        UpdatedAt = DateTime.UtcNow;
    }
}