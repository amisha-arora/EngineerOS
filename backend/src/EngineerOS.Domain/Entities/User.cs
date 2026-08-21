namespace EngineerOS.Domain.Entities;

public sealed class User
{
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<Repository> _repositories = [];

    private User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens.AsReadOnly();

    public IReadOnlyCollection<Repository> Repositories =>
    _repositories.AsReadOnly();
}