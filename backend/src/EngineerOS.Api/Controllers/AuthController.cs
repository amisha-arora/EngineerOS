using EngineerOS.Application.Features.Authentication.Login;
using EngineerOS.Application.Features.Authentication.Register;
using EngineerOS.Application.Features.Authentication.Refresh;
using EngineerOS.Application.Features.Authentication.Logout;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EngineerOS.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserService _registerUserService;
    private readonly LoginUserService _loginUserService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly LogoutService _logoutService;

    public AuthController(
        RegisterUserService registerUserService,
        LoginUserService loginUserService,
        RefreshTokenService refreshTokenService,
        LogoutService logoutService)
    {
        _registerUserService = registerUserService;
        _loginUserService = loginUserService;
        _refreshTokenService = refreshTokenService;
        _logoutService = logoutService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _registerUserService.RegisterAsync(
                    request,
                    cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (ValidationException exception)
        {
            var errors = exception.Errors
                .Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                });

            return BadRequest(new
            {
                message = "Validation failed.",
                errors
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _loginUserService.LoginAsync(
                    request,
                    cancellationToken);

            return Ok(response);
        }
        catch (ValidationException exception)
        {
            var errors = exception.Errors
                .Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                });

            return BadRequest(new
            {
                message = "Validation failed.",
                errors
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
    RefreshTokenRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _refreshTokenService.RefreshAsync(
                    request,
                    cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token."
            });
        }
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    LogoutRequest request,
    CancellationToken cancellationToken)
    {
        await _logoutService.LogoutAsync(
            request,
            cancellationToken);

        return NoContent();
    }
}