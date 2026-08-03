using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration :
    IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        builder.Property(refreshToken => refreshToken.CreatedAtUtc)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ExpiresAtUtc)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.RevokedAtUtc);

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.Ignore(refreshToken => refreshToken.IsExpired);

        builder.Ignore(refreshToken => refreshToken.IsRevoked);

        builder.Ignore(refreshToken => refreshToken.IsActive);
    }
}