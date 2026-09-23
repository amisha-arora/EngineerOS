using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class CodeDependencyConfiguration
    : IEntityTypeConfiguration<CodeDependency>
{
    public void Configure(EntityTypeBuilder<CodeDependency> builder)
    {
        builder.ToTable("code_dependencies");

        builder.HasKey(dependency => dependency.Id);

        builder.Property(dependency => dependency.SourceTypeName)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(dependency => dependency.TargetTypeName)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(dependency => dependency.DependencyType)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<Repository>()
            .WithMany()
            .HasForeignKey(dependency => dependency.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<CodeClass>()
            .WithMany()
            .HasForeignKey(dependency => dependency.SourceCodeClassId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<CodeClass>()
            .WithMany()
            .HasForeignKey(dependency => dependency.TargetCodeClassId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(dependency => dependency.RepositoryId);
    }
}