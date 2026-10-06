
namespace EngineerOS.Domain.Entities;

public sealed class Embedding
{
    public Guid Id { get; private set; }

    public Guid? DocumentChunkId { get; private set; }

    public Guid? CodeChunkId { get; private set; }

    public float[] Vector { get; private set; }

    public string Model { get; private set; }

    public int Dimensions { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Embedding()
    {
        Vector = null!;
        Model = null!;
    }

    public Embedding(
        Guid? documentChunkId,
        Guid? codeChunkId,
        float[] vector,
        string model)
    {
        if (documentChunkId is null &&
            codeChunkId is null)
        {
            throw new ArgumentException(
                "An embedding must belong to a document chunk or code chunk.");
        }

        if (documentChunkId is not null &&
            codeChunkId is not null)
        {
            throw new ArgumentException(
                "An embedding cannot belong to both a document chunk and a code chunk.");
        }

        Id = Guid.NewGuid();

        DocumentChunkId = documentChunkId;
        CodeChunkId = codeChunkId;

        Vector = vector;
        Model = model;
        Dimensions = vector.Length;

        CreatedAt = DateTime.UtcNow;
    }
}