using EngineerOS.Application.Knowledge.DTOs;

namespace EngineerOS.Application.Abstractions.Knowledge;

public interface IRepositoryQuestionAnsweringService
{
    Task<RepositoryAnswer> AskAsync(
        Guid repositoryId,
        string question,
        CancellationToken cancellationToken = default);
}