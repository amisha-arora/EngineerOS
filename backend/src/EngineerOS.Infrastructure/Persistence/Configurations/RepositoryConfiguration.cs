using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class RepositoryConfiguration
    : IEntityTypeConfiguration<Repository>
{
    public void Configure(
        EntityTypeBuilder<Repository> builder)
    {
        builder.ToTable("repositories");

        builder.HasKey(repository => repository.Id);

        builder.Property(repository => repository.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(repository => repository.Url)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(repository => repository.CreatedAtUtc)
            .IsRequired();

        builder.Property(repository => repository.UserId)
            .IsRequired();

        builder.HasOne(repository => repository.User)
            .WithMany(user => user.Repositories)
            .HasForeignKey(repository => repository.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(repository => new
        {
            repository.UserId,
            repository.Url
        })
        .IsUnique();
    }
}