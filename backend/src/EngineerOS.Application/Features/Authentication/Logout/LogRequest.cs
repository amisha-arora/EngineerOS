namespace EngineerOS.Application.Features.Authentication.Logout;

public sealed record LogoutRequest(
    string RefreshToken);