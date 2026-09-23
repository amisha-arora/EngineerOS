namespace EngineerOS.Application.Abstractions.Extraction;

public sealed record CSharpDependency(
    string File,
    string SourceType,
    string TargetType,
    string Relationship,
    int LineNumber);