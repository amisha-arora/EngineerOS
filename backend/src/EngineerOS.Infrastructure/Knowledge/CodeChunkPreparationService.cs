using System.Security.Cryptography;
using System.Text;
using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Domain.Entities;
using EngineerOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Knowledge;

public sealed class CodeChunkPreparationService
    : ICodeChunkPreparationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _storageRoot;

    public CodeChunkPreparationService(
        ApplicationDbContext dbContext,
        string storageRoot)
    {
        _dbContext = dbContext;
        _storageRoot = storageRoot;
    }

    public async Task PrepareAsync(
        Guid repositoryId,
        CancellationToken cancellationToken = default)
    {

        await _dbContext.CodeChunks
            .Where(x => x.RepositoryId == repositoryId)
            .ExecuteDeleteAsync(cancellationToken);

        // Get persisted C# repository files.
        var files = await _dbContext
            .Set<RepositoryFile>()
            .Where(x =>
                x.RepositoryId == repositoryId &&
                x.Type == "CSharp")
            .ToListAsync(cancellationToken);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await PrepareFileAsync(
                repositoryId,
                file,
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task PrepareFileAsync(
        Guid repositoryId,
        RepositoryFile file,
        CancellationToken cancellationToken)
    {
        // Reuse Phase 3 static-analysis results.
        var classes = await _dbContext
            .Set<CodeClass>()
            .Where(x => x.RepositoryFileId == file.Id)
            .ToListAsync(cancellationToken);

        var chunkIndex = 0;

        foreach (var codeClass in classes)
        {
            var methods = await _dbContext
                .Set<CodeMethod>()
                .Where(x => x.CodeClassId == codeClass.Id)
                .ToListAsync(cancellationToken);

            foreach (var method in methods)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var codeChunk =
                    await CreateMethodChunkAsync(
                        repositoryId,
                        file,
                        codeClass,
                        method,
                        chunkIndex++,
                        cancellationToken);

                _dbContext
                    .Set<CodeChunk>()
                    .Add(codeChunk);
            }
        }
    }

    private async Task<CodeChunk> CreateMethodChunkAsync(
        Guid repositoryId,
        RepositoryFile file,
        CodeClass codeClass,
        CodeMethod method,
        int chunkIndex,
        CancellationToken cancellationToken)
    {
        var extractedDirectory = Path.Combine(
            _storageRoot,
            "repositories",
            repositoryId.ToString(),
            "extracted");

        var normalizedPath = file.Path
            .Replace(
                '/',
                Path.DirectorySeparatorChar)
            .Replace(
                '\\',
                Path.DirectorySeparatorChar);

        var fullPath = Path.Combine(
            extractedDirectory,
            normalizedPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"Repository source file '{file.Path}' was not found.",
                fullPath);
        }

        var lines = await File.ReadAllLinesAsync(
            fullPath,
            cancellationToken);

        var startLine = Math.Max(
            method.LineNumber,
            1);

        if (startLine > lines.Length)
        {
            throw new InvalidOperationException(
                $"Method '{method.Name}' starts at line {startLine}, " +
                $"but file '{file.Path}' only contains {lines.Length} lines.");
        }

        // Temporary approximation.
        // Later Roslyn can provide the exact method end line.
        var endLine = Math.Min(
            startLine + 100,
            lines.Length);

        var code = string.Join(
            Environment.NewLine,
            lines[(startLine - 1)..endLine]);

        var hash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(code)));

        return new CodeChunk(
            repositoryId,
            file.Id,
            codeClass.Id,
            method.Id,
            chunkIndex,
            startLine,
            endLine,
            method.Name,
            "Method",
            hash);
    }
}