using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}