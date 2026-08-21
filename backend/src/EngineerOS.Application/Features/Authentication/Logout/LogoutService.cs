using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;

namespace EngineerOS.Application.Features.Authentication.Logout;

public sealed class LogoutService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public LogoutService(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _refreshTokenGenerator.Hash(
                request.RefreshToken);

        var refreshToken =
            await _refreshTokenRepository
                .GetByTokenHashAsync(
                    tokenHash,
                    cancellationToken);

        if (refreshToken is null ||
            !refreshToken.IsActive)
        {
            return;
        }

        refreshToken.Revoke();

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);
    }
}