namespace EngineerOS.Application.Abstractions.Extraction;

public sealed record CSharpType(
    string File,
    string Namespace,
    string Name,
    string Kind,
    string Visibility,
    string? Modifier);