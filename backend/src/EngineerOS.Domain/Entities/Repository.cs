namespace EngineerOS.Domain.Entities;

public sealed class Repository
{
    private readonly List<RepositoryAnalysis> _analyses = new();

    private Repository()
    {
    }

    public Repository(
        Guid userId,
        string name,
        string url)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Url = url;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Url { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public User User { get; private set; } = null!;

    public IReadOnlyCollection<RepositoryAnalysis> Analyses =>
        _analyses.AsReadOnly();
}