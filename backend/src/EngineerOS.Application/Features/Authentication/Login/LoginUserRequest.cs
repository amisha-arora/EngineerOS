namespace EngineerOS.Application.Features.Authentication.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password);