namespace EngineerOS.Domain.Entities;

public sealed class CodeMethod
{
    public Guid Id { get; private set; }

    public Guid CodeClassId { get; private set; }

    public string Name { get; private set; }

    public string ReturnType { get; private set; }

    public string AccessModifier { get; private set; }

    public int LineNumber { get; private set; }

    private CodeMethod()
    {
        Name = null!;
        ReturnType = null!;
        AccessModifier = null!;
    }

    public CodeMethod(
        Guid codeClassId,
        string name,
        string returnType,
        string accessModifier,
        int lineNumber)
    {
        Id = Guid.NewGuid();
        CodeClassId = codeClassId;
        Name = name;
        ReturnType = returnType;
        AccessModifier = accessModifier;
        LineNumber = lineNumber;
    }
}