using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Extraction;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Application.Abstractions.Storage;
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
        // assigning is done here because the constructor parameters are being passed in and we want to store them in the private fields for later use in the class methods.
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

            var types = await _cSharpTypeExtractor.ExtractAsync(
                repositoryId,
                cancellationToken);

            var methods = await _cSharpMethodExtractor.ExtractAsync(
                repositoryId,
                cancellationToken);

            var dependencies =
                await _cSharpDependencyExtractor.ExtractAsync(
                    repositoryId,
                    cancellationToken);

            await _repositoryAnalysisWriter.ReplaceMetadataAsync(
                repositoryId,
                files,
                types,
                methods,
                dependencies,
                cancellationToken);

            analysis.MarkCompleted();

            await _repositoryAnalysisRepository.SaveChangesAsync(
                cancellationToken);

            return new AnalyzeRepositoryResponse(
                analysis.Id,
                analysis.Status,
                files.Count,
                types.Count,
                methods.Count,
                dependencies.Count);
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