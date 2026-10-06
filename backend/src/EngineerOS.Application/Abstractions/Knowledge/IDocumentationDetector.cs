using EngineerOS.Domain.Enums;

namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IDocumentationDetector
{
    bool IsDocumentation(string? extension);

    DocumentType? GetDocumentType(string? extension);
}