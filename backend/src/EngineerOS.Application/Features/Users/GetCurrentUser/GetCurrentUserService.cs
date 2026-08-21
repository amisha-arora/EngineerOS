using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Users.GetCurrentUser;

public sealed class GetCurrentUserService
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserService(
        ICurrentUserService currentUser,
        IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<GetCurrentUserResponse?> GetAsync(
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = await _userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new GetCurrentUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email);
    }
}