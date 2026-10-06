using EngineerOS.Domain.Enums;

namespace EngineerOS.Application.Knowledge.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public DocumentType Type { get; set; }
}