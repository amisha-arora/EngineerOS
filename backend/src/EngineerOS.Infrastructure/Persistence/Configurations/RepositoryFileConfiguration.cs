using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class RepositoryFileConfiguration
    : IEntityTypeConfiguration<RepositoryFile>
{
    public void Configure(EntityTypeBuilder<RepositoryFile> builder)
    {
        builder.ToTable("repository_files");

        builder.HasKey(file => file.Id);

        builder.Property(file => file.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(file => file.Path)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(file => file.Extension)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(file => file.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(file => file.ParentDirectory)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasOne<Repository>()
            .WithMany()
            .HasForeignKey(file => file.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(file => new
        {
            file.RepositoryId,
            file.Path
        })
        .IsUnique();
    }
}