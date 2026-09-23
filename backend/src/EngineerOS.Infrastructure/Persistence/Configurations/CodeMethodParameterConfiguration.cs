using EngineerOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngineerOS.Infrastructure.Persistence.Configurations;

public sealed class CodeMethodParameterConfiguration
    : IEntityTypeConfiguration<CodeMethodParameter>
{
    public void Configure(EntityTypeBuilder<CodeMethodParameter> builder)
    {
        builder.ToTable("code_method_parameters");

        builder.HasKey(parameter => parameter.Id);

        builder.Property(parameter => parameter.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(parameter => parameter.Type)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasOne<CodeMethod>()
            .WithMany()
            .HasForeignKey(parameter => parameter.CodeMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(parameter => new
        {
            parameter.CodeMethodId,
            parameter.Position
        })
        .IsUnique();
    }
}