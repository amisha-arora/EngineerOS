using System.IO.Compression;
using EngineerOS.Application.Abstractions.Extraction;

namespace EngineerOS.Infrastructure.Extraction;

public sealed class RepositoryExtractor
    : IRepositoryExtractor
{
    private readonly string _storageRoot;

    public RepositoryExtractor(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public Task<string> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var repositoryDirectory =
            Path.Combine(
                _storageRoot,
                "repositories",
                repositoryId.ToString());

        var zipPath =
            Path.Combine(
                repositoryDirectory,
                "repository.zip");

        var extractionDirectory =
            Path.Combine(
                repositoryDirectory,
                "extracted");

        if (!File.Exists(zipPath))
        {
            throw new FileNotFoundException(
                "Repository ZIP file was not found.",
                zipPath);
        }

        Directory.CreateDirectory(
            extractionDirectory);

        var fullExtractionPath =
            Path.GetFullPath(
                extractionDirectory);

        try
        {
            using var archive =
                ZipFile.OpenRead(zipPath);

            foreach (var entry in archive.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var destinationPath =
                    Path.Combine(
                        extractionDirectory,
                        entry.FullName);

                var fullDestinationPath =
                    Path.GetFullPath(
                        destinationPath);

                // Prevent ZIP path traversal.
                if (!fullDestinationPath.StartsWith(
                        fullExtractionPath +
                        Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        "ZIP entry attempts to extract outside the repository directory.");
                }

                // Directory entry.
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(
                        fullDestinationPath);

                    continue;
                }

                var parentDirectory =
                    Path.GetDirectoryName(
                        fullDestinationPath);

                if (parentDirectory is not null)
                {
                    Directory.CreateDirectory(
                        parentDirectory);
                }

                // Do not overwrite an existing file.
                entry.ExtractToFile(
                    fullDestinationPath,
                    overwrite: false);
            }
        }
        catch (InvalidDataException)
        {
            throw new InvalidDataException(
                "The uploaded repository is not a valid ZIP file.");
        }

        return Task.FromResult(
            extractionDirectory);
    }
}

