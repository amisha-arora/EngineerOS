
using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Application.Abstractions.RepositoryFiles;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Repositories.Analyze;

public sealed class AnalyzeRepositoryService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly IRepositoryAnalysisRepository _repositoryAnalysisRepository;
    private readonly IRepositoryAnalysisWriter _repositoryAnalysisWriter;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepositoryFileReader _repositoryFileReader;
    private readonly ICSharpTypeExtractor _cSharpTypeExtractor;
    private readonly ICSharpMethodExtractor _cSharpMethodExtractor;
    private readonly ICSharpDependencyExtractor _cSharpDependencyExtractor;

    public AnalyzeRepositoryService(
        IRepositoryRepository repositoryRepository,
        IRepositoryAnalysisRepository repositoryAnalysisRepository,
        IRepositoryAnalysisWriter repositoryAnalysisWriter,
        ICurrentUserService currentUserService,
        IRepositoryFileReader repositoryFileReader,
        ICSharpTypeExtractor cSharpTypeExtractor,
        ICSharpMethodExtractor cSharpMethodExtractor,
        ICSharpDependencyExtractor cSharpDependencyExtractor)
    {
        _repositoryRepository = repositoryRepository;
        _repositoryAnalysisRepository = repositoryAnalysisRepository;
        _repositoryAnalysisWriter = repositoryAnalysisWriter;
        _currentUserService = currentUserService;
        _repositoryFileReader = repositoryFileReader;
        _cSharpTypeExtractor = cSharpTypeExtractor;
        _cSharpMethodExtractor = cSharpMethodExtractor;
        _cSharpDependencyExtractor = cSharpDependencyExtractor;
    }

    public async Task<AnalyzeRepositoryResponse?> AnalyzeAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var repository = await _repositoryRepository.GetByIdAsync(
            repositoryId,
            cancellationToken);

        if (repository is null ||
            repository.UserId != _currentUserService.UserId)
        {
            return null;
        }

        var analysis = new RepositoryAnalysis(
            repositoryId,
            "Pending");

        await _repositoryAnalysisRepository.AddAsync(
            analysis,
            cancellationToken);

        await _repositoryAnalysisRepository.SaveChangesAsync(
            cancellationToken);

        try
        {
            var files = await _repositoryFileReader.GetFilesAsync(
                repositoryId,
                cancellationToken);

            var extractedTypes = await _cSharpTypeExtractor.ExtractAsync(
                repositoryId,
                cancellationToken);

            var extractedMethods = await _cSharpMethodExtractor.ExtractAsync(
                repositoryId,
                cancellationToken);

            var extractedDependencies =
                await _cSharpDependencyExtractor.ExtractAsync(
                    repositoryId,
                    cancellationToken);

            var codeClasses = extractedTypes
                .Select(type =>
                {
                    var file = files.FirstOrDefault(
                        file => string.Equals(
                            file.Path,
                            type.File,
                            StringComparison.OrdinalIgnoreCase));

                    if (file is null)
                    {
                        throw new InvalidOperationException(
                            $"Repository file '{type.File}' was not found.");
                    }

                    return new CodeClass(
                        file.Id,
                        type.Name,
                        type.Namespace,
                        type.Kind,
                        type.Visibility,
                        type.Modifier);
                })
                .ToList();

            var codeMethods = extractedMethods
                .Select(method =>
                {
                    var codeClass = codeClasses.FirstOrDefault(
                        type => string.Equals(
                            type.Name,
                            method.ContainingType,
                            StringComparison.Ordinal));

                    if (codeClass is null)
                    {
                        throw new InvalidOperationException(
                            $"Containing type '{method.ContainingType}' " +
                            $"was not found for method '{method.Name}'.");
                    }

                    return new CodeMethod(
                        codeClass.Id,
                        method.Name,
                        method.ReturnType,
                        method.Visibility,
                        method.LineNumber);
                })
                .ToList();

            var codeDependencies = extractedDependencies
                .Select(dependency =>
                {
                    var sourceClass = codeClasses.FirstOrDefault(
                        type => string.Equals(
                            type.Name,
                            dependency.SourceType,
                            StringComparison.Ordinal));

                    var targetClass = codeClasses.FirstOrDefault(
                        type => string.Equals(
                            type.Name,
                            dependency.TargetType,
                            StringComparison.Ordinal));

                    return new CodeDependency(
                        repositoryId,
                        sourceClass?.Id,
                        dependency.SourceType,
                        targetClass?.Id,
                        dependency.TargetType,
                        dependency.Relationship,
                        dependency.LineNumber);
                })
                .ToList();

            await _repositoryAnalysisWriter.ReplaceMetadataAsync(
                repositoryId,
                files,
                codeClasses,
                codeMethods,
                codeDependencies,
                cancellationToken);

            analysis.MarkCompleted();

            await _repositoryAnalysisRepository.SaveChangesAsync(
                cancellationToken);

            return new AnalyzeRepositoryResponse(
                analysis.Id,
                analysis.Status,
                files.Count,
                codeClasses.Count,
                codeMethods.Count,
                codeDependencies.Count);
        }
        catch
        {
            analysis.MarkFailed();

            await _repositoryAnalysisRepository.SaveChangesAsync(
                CancellationToken.None);

            throw;
        }
    }
}
