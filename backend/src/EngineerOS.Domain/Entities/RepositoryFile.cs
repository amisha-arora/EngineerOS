namespace EngineerOS.Domain.Entities;

public sealed class RepositoryFile
{
    public Guid Id { get; private set; }

    public Guid RepositoryId { get; private set; }

    public string Name { get; private set; }

    public string Path { get; private set; }

    public string Extension { get; private set; }

    public string Type { get; private set; }

    public long Size { get; private set; }

    public string ParentDirectory { get; private set; }

    private RepositoryFile()
    {
        Name = null!;
        Path = null!;
        Extension = null!;
        Type = null!;
        ParentDirectory = null!;
    }

    public RepositoryFile(
        Guid repositoryId,
        string name,
        string path,
        string extension,
        string type,
        long size,
        string parentDirectory)
    {
        Id = Guid.NewGuid();
        RepositoryId = repositoryId;
        Name = name;
        Path = path;
        Extension = extension;
        Type = type;
        Size = size;
        ParentDirectory = parentDirectory;
    }
}