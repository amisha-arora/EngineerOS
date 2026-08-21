namespace EngineerOS.Application.Features.Users.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);