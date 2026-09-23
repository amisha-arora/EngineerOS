using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class CodeMethodConfiguration
    : IEntityTypeConfiguration<CodeMethod>
{
    public void Configure(EntityTypeBuilder<CodeMethod> builder)
    {
        builder.ToTable("code_methods");

        builder.HasKey(method => method.Id);

        builder.Property(method => method.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(method => method.ReturnType)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(method => method.AccessModifier)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<CodeClass>()
            .WithMany()
            .HasForeignKey(method => method.CodeClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(method => method.CodeClassId);
    }
}