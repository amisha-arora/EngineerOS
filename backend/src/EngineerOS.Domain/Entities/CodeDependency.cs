namespace EngineerOS.Domain.Entities;

public sealed class CodeDependency
{
    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public Guid? SourceCodeClassId { get; private set; }

    public string SourceTypeName { get; private set; }

    public Guid? TargetCodeClassId { get; private set; }

    public string TargetTypeName { get; private set; }

    public string DependencyType { get; private set; }

    public int LineNumber { get; private set; }

    private CodeDependency()
    {
        SourceTypeName = null!;
        TargetTypeName = null!;
        DependencyType = null!;
    }

    public CodeDependency(
        Guid repositoryId,
        Guid? sourceCodeClassId,
        string sourceTypeName,
        Guid? targetCodeClassId,
        string targetTypeName,
        string dependencyType,
        int lineNumber)
    {
        Id = Guid.NewGuid();
        RepositoryId = repositoryId;
        SourceCodeClassId = sourceCodeClassId;
        SourceTypeName = sourceTypeName;
        TargetCodeClassId = targetCodeClassId;
        TargetTypeName = targetTypeName;
        DependencyType = dependencyType;
        LineNumber = lineNumber;
    }
}