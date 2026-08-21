using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;

namespace EngineerOS.Application.Features.Authentication.Refresh;

public sealed class RefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenGenerator refreshTokenGenerator,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RefreshTokenResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _refreshTokenGenerator.Hash(
                request.RefreshToken);

        var existingToken =
            await _refreshTokenRepository
                .GetByTokenHashAsync(
                    tokenHash,
                    cancellationToken);

        if (existingToken is null ||
            !existingToken.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        existingToken.Revoke();

        var newRawRefreshToken =
            _refreshTokenGenerator.Generate();

        var newRefreshTokenHash =
            _refreshTokenGenerator.Hash(
                newRawRefreshToken);

        var newRefreshToken =
            new RefreshToken(
                existingToken.UserId,
                newRefreshTokenHash,
                DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(
            newRefreshToken,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        var newAccessToken =
            _jwtTokenGenerator.GenerateAccessToken(
                existingToken.User);

        return new RefreshTokenResponse(
            newAccessToken,
            newRawRefreshToken);
    }
}