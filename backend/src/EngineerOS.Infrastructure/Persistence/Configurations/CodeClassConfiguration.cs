using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class CodeClassConfiguration
    : IEntityTypeConfiguration<CodeClass>
{
    public void Configure(EntityTypeBuilder<CodeClass> builder)
    {
        builder.ToTable("code_classes");

        builder.HasKey(codeClass => codeClass.Id);

        builder.Property(codeClass => codeClass.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(codeClass => codeClass.Namespace)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(codeClass => codeClass.Kind)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(codeClass => codeClass.AccessModifier)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(codeClass => codeClass.Modifier)
            .HasMaxLength(100);

        builder.HasOne<RepositoryFile>()
            .WithMany()
            .HasForeignKey(codeClass => codeClass.RepositoryFileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(codeClass => codeClass.RepositoryFileId);
    }
}