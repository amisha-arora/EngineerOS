namespace EngineerOS.Domain.Entities;

public sealed class CodeMethodParameter
{
    public Guid Id { get; private set; }

    public Guid CodeMethodId { get; private set; }

    public string Name { get; private set; }

    public string Type { get; private set; }

    public int Position { get; private set; }

    private CodeMethodParameter()
    {
        Name = null!;
        Type = null!;
    }

    public CodeMethodParameter(
        Guid codeMethodId,
        string name,
        string type,
        int position)
    {
        Id = Guid.NewGuid();
        CodeMethodId = codeMethodId;
        Name = name;
        Type = type;
        Position = position;
    }
}