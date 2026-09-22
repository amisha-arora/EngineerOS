namespace EngineerOS.Application.Abstractions.Extraction;

public sealed record CSharpMethod(
    string File,
    string ContainingType,
    string Name,
    string ReturnType,
    string Visibility,
    IReadOnlyList<CSharpMethodParameter> Parameters,
    int LineNumber);