namespace EngineerOS.Domain.Entities;

public sealed class CodeClass
{
    public Guid Id { get; private set; }

    public Guid RepositoryFileId { get; private set; }

    public string Name { get; private set; }

    public string Namespace { get; private set; }

    public string Kind { get; private set; }

    public string AccessModifier { get; private set; }

    public string? Modifier { get; private set; }

    private CodeClass()
    {
        Name = null!;
        Namespace = null!;
        Kind = null!;
        AccessModifier = null!;
    }

    public CodeClass(
        Guid repositoryFileId,
        string name,
        string @namespace,
        string kind,
        string accessModifier,
        string? modifier)
    {
        Id = Guid.NewGuid();
        RepositoryFileId = repositoryFileId;
        Name = name;
        Namespace = @namespace;
        Kind = kind;
        AccessModifier = accessModifier;
        Modifier = modifier;
    }
}