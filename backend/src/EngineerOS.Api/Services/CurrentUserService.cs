using System.Security.Claims;
using EngineerOS.Application.Abstractions.Authentication;

namespace EngineerOS.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "User identifier is missing.");
            }

            return id;
        }
    }
}