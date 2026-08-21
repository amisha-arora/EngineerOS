using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngineerOS.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            user => user.Email == email,
            cancellationToken);
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == email,
            cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
    public Task<User?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == id,
                cancellationToken);
    }
}