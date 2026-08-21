namespace EngineerOS.Domain.Entities;

public sealed class RepositoryAnalysis
{
    private RepositoryAnalysis()
    {
    }

    public RepositoryAnalysis(
        Guid repositoryId,
        string status)
    {
        Id = Guid.NewGuid();
        RepositoryId = repositoryId;
        Status = status;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public string Status { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public Repository Repository { get; private set; } = null!;
}