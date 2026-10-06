using EngineerOS.Application.Abstractions.Knowledge;
using UglyToad.PdfPig;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class PdfContentExtractor : IContentExtractor
{
    public bool CanExtract(string extension)
    {
        return string.Equals(
            extension,
            ".pdf",
            StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        //The using statement ensures that once the PDF is read, the file memory is safely cleaned up and closed
        using var document = PdfDocument.Open(filePath);

        var text = string.Join(
            Environment.NewLine,
            document.GetPages()
                .Select(page => page.Text));

        return Task.FromResult(text);
    }
}