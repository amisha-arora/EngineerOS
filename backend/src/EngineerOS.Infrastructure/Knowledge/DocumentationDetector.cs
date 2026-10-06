using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Enums;

namespace EngineerOS.Infrastructure.Knowledge;

public class DocumentationDetector : IDocumentationDetector
{
    public bool IsDocumentation(string? extension)
    {
        return GetDocumentType(extension) is not null;
    }

    public DocumentType? GetDocumentType(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return null;

        return extension.Trim().ToLowerInvariant() switch
        {
            ".md" => DocumentType.Markdown,
            ".txt" => DocumentType.Text,
            ".pdf" => DocumentType.Pdf,
            ".docx" => DocumentType.Docx,
            _ => null
        };
    }
}