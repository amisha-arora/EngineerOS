namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IDocumentChunker
{
    IReadOnlyList<DocumentChunkResult> Chunk(
        string content);
}