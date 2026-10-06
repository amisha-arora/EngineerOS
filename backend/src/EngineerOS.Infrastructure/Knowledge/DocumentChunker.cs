using System.Text;
using EngineerOS.Application.Abstractions.Knowledge;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class DocumentChunker : IDocumentChunker
{
    public IReadOnlyList<DocumentChunkResult> Chunk(
        string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Array.Empty<DocumentChunkResult>();
        }

        var lines = content.Replace(
                "\r\n",
                "\n")
            .Split('\n');

        var chunks = new List<DocumentChunkResult>();

        var currentSection = "General";

        var currentContent = new StringBuilder();

        foreach (var line in lines)
        {
            if (IsHeading(line))
            {
                AddChunk(
                    chunks,
                    currentContent,
                    currentSection);

                currentSection = ExtractHeading(line);

                continue;
            }

            currentContent.AppendLine(line);
        }

        AddChunk(
            chunks,
            currentContent,
            currentSection);

        return chunks;
    }

    private static bool IsHeading(string line)
    {
        return line.StartsWith("#");
    }

    private static string ExtractHeading(string line)
    {
        return line.TrimStart('#').Trim();
    }

    private static void AddChunk(
        List<DocumentChunkResult> chunks,
        StringBuilder content,
        string section)
    {
        var text = content
            .ToString()
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            content.Clear();
            return;
        }

        chunks.Add(
            new DocumentChunkResult(
                chunks.Count,
                text,
                section,
                null));

        content.Clear();
    }
}