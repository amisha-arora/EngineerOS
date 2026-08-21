namespace EngineerOS.Application.Features.Authentication.Register;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);