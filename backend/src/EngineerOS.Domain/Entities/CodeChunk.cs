namespace EngineerOS.Domain.Entities;

public sealed class CodeChunk
{
    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public Guid RepositoryFileId { get; private set; }

    public Guid? CodeClassId { get; private set; }

    public Guid? CodeMethodId { get; private set; }

    public int ChunkIndex { get; private set; }

    public int StartLine { get; private set; }

    public int EndLine { get; private set; }

    public string? SymbolName { get; private set; }

    public string? SymbolType { get; private set; }

    public string? ContentHash { get; private set; }

    private CodeChunk()
    {
    }

    public CodeChunk(
        Guid repositoryId,
        Guid repositoryFileId,
        Guid? codeClassId,
        Guid? codeMethodId,
        int chunkIndex,
        int startLine,
        int endLine,
        string? symbolName,
        string? symbolType,
        string? contentHash)
    {
        Id = Guid.NewGuid();

        RepositoryId = repositoryId;
        RepositoryFileId = repositoryFileId;

        CodeClassId = codeClassId;
        CodeMethodId = codeMethodId;

        ChunkIndex = chunkIndex;

        StartLine = startLine;
        EndLine = endLine;

        SymbolName = symbolName;
        SymbolType = symbolType;

        ContentHash = contentHash;
    }
}