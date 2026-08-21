using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;
using FluentValidation;

namespace EngineerOS.Application.Features.Authentication.Register;

public sealed class RegisterUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserRequest> _validator;

    public RegisterUserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserRequest> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<RegisterUserResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors);
        }

        var normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var emailExists =
            await _userRepository.EmailExistsAsync(
                normalizedEmail,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = new User(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            normalizedEmail,
            passwordHash);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        return new RegisterUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email);
    }
}