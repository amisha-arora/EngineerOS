

namespace EngineerOS.Application.Features.Repositories.Create;

public sealed record CreateRepositoryRequest(
    string Name,
    string Url);