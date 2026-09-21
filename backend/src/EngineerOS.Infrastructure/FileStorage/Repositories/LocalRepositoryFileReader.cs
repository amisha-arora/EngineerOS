
using EngineerOS.Application.Abstractions.Storage;

namespace EngineerOS.Infrastructure.FileStorage.Repositories;

public sealed class LocalRepositoryFileReader : IRepositoryFileReader
{
    private readonly string _storageRoot;

    public LocalRepositoryFileReader(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public Task<IReadOnlyList<RepositoryFile>> GetFilesAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var extractedDirectory = Path.Combine(
            _storageRoot,
            "repositories",
            repositoryId.ToString(),
            "extracted");

        if (!Directory.Exists(extractedDirectory))
        {
            return Task.FromResult<IReadOnlyList<RepositoryFile>>([]);
        }

        var files = new List<RepositoryFile>();

        foreach (var fullPath in Directory.EnumerateFiles(
                     extractedDirectory,
                     "*",
                     SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var name = Path.GetFileName(fullPath);

            var path = Path.GetRelativePath(
                extractedDirectory,
                fullPath).Replace('\\', '/');

            var extension = Path.GetExtension(name);

            var parentDirectory = Path.GetDirectoryName(path)?
                .Replace('\\', '/')
                ?? string.Empty;

            var size = new FileInfo(fullPath).Length;

            files.Add(new RepositoryFile(
                name,
                path,
                extension,
                GetFileType(extension),
                size,
                parentDirectory));
        }

        files.Sort((left, right) =>
            StringComparer.Ordinal.Compare(left.Path, right.Path));

        return Task.FromResult<IReadOnlyList<RepositoryFile>>(files);
    }

    private static string GetFileType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".cs" => "CSharp",
            ".csproj" => "CSharpProject",
            ".sln" => "Solution",
            ".json" => "Json",
            ".md" => "Markdown",
            ".ts" => "TypeScript",
            ".tsx" => "TypeScriptReact",
            ".js" => "JavaScript",
            ".jsx" => "JavaScriptReact",
            ".html" => "Html",
            ".css" => "Css",
            ".sql" => "Sql",
            _ => "Other"
        };
    }
}