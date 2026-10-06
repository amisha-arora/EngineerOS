using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class DocxContentExtractor : IContentExtractor
{
    public bool CanExtract(string extension)
    {
        return string.Equals(
            extension,
            ".docx",
            StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var document =
            WordprocessingDocument.Open(filePath, false);

        var body = document.MainDocumentPart?.Document?.Body;

        if (body is null)
        {
            return Task.FromResult(string.Empty);
        }

        var paragraphs = body
            .Descendants<Paragraph>()
            .Select(paragraph =>
                string.Concat(
                    paragraph
                        .Descendants<Text>()
                        .Select(text => text.Text)))
            .Where(text => !string.IsNullOrWhiteSpace(text));

        var result = string.Join(
            Environment.NewLine,
            paragraphs);

        return Task.FromResult(result);
    }
}