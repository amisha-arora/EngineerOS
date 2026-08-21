using EngineerOS.Application.Features.Users.GetCurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngineerOS.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly GetCurrentUserService _getCurrentUserService;

    public UsersController(
        GetCurrentUserService getCurrentUserService)
    {
        _getCurrentUserService = getCurrentUserService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(
        CancellationToken cancellationToken)
    {
        var user =
            await _getCurrentUserService.GetAsync(
                cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}