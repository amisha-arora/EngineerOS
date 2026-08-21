namespace EngineerOS.Application.Features.Authentication.Register;

public sealed record RegisterUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);