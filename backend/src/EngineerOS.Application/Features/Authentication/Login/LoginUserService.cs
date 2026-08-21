using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Authentication.Login;

public sealed class LoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginUserRequest> _validator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public LoginUserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<LoginUserRequest> validator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _validator = validator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }

    public async Task<LoginUserResponse> LoginAsync(
        LoginUserRequest request,
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

        var user =
            await _userRepository.GetByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordIsValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var accessToken =
            _jwtTokenGenerator.GenerateAccessToken(user);
        var rawRefreshToken =
            _refreshTokenGenerator.Generate();

        var refreshTokenHash =
            _refreshTokenGenerator.Hash(rawRefreshToken);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        return new LoginUserResponse(accessToken, rawRefreshToken);
    }
}