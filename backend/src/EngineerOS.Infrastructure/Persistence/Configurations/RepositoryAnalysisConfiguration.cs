using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class RepositoryAnalysisConfiguration
    : IEntityTypeConfiguration<RepositoryAnalysis>
{
    public void Configure(
        EntityTypeBuilder<RepositoryAnalysis> builder)
    {
        builder.ToTable("repository_analyses");

        builder.HasKey(analysis => analysis.Id);

        builder.Property(analysis => analysis.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(analysis => analysis.CreatedAtUtc)
            .IsRequired();

        builder.Property(analysis => analysis.CompletedAtUtc);

        builder.Property(analysis => analysis.RepositoryId)
            .IsRequired();

        builder.HasOne(analysis => analysis.Repository)
            .WithMany(repository => repository.Analyses)
            .HasForeignKey(analysis => analysis.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}