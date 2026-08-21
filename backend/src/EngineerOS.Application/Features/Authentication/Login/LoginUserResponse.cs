namespace EngineerOS.Application.Features.Authentication.Login;

public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken);