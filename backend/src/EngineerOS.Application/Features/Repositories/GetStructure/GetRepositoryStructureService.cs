using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Repositories.GetStructure;

public sealed class GetRepositoryStructureService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly IRepositoryAnalysisReader _repositoryAnalysisReader;
    private readonly ICurrentUserService _currentUserService;

    public GetRepositoryStructureService(
        IRepositoryRepository repositoryRepository,
        IRepositoryAnalysisReader repositoryAnalysisReader,
        ICurrentUserService currentUserService)
    {
        _repositoryRepository = repositoryRepository;
        _repositoryAnalysisReader = repositoryAnalysisReader;
        _currentUserService = currentUserService;
    }

    public async Task<GetRepositoryStructureResponse?> GetAsync(
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

        var files = await _repositoryAnalysisReader.GetFilesAsync(
            repositoryId,
            cancellationToken);

        var classes = await _repositoryAnalysisReader.GetClassesAsync(
            files.Select(file => file.Id).ToList(),
            cancellationToken);

        var methods = await _repositoryAnalysisReader.GetMethodsAsync(
            classes.Select(codeClass => codeClass.Id).ToList(),
            cancellationToken);

        var responseFiles = files.Select(file =>
        {
            var fileClasses = classes
                .Where(codeClass =>
                    codeClass.RepositoryFileId == file.Id)
                .Select(codeClass =>
                {
                    var classMethods = methods
                        .Where(method =>
                            method.CodeClassId == codeClass.Id)
                        .Select(method =>
                            new RepositoryStructureMethodResponse(
                                method.Id,
                                method.Name,
                                method.ReturnType,
                                method.AccessModifier,
                                method.LineNumber))
                        .ToList();

                    return new RepositoryStructureTypeResponse(
                        codeClass.Id,
                        codeClass.Name,
                        codeClass.Namespace,
                        codeClass.Kind,
                        codeClass.AccessModifier,
                        codeClass.Modifier,
                        classMethods);
                })
                .ToList();

            return new RepositoryStructureFileResponse(
                file.Id,
                file.Name,
                file.Path,
                file.Extension,
                file.Type,
                file.Size,
                file.ParentDirectory,
                fileClasses);
        })
        .ToList();

        return new GetRepositoryStructureResponse(
            repositoryId,
            responseFiles);
    }
}