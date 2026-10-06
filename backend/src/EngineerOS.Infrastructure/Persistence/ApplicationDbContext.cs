using Microsoft.EntityFrameworkCore;
using EngineerOS.Domain.Entities;
using Pgvector;

namespace EngineerOS.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    public DbSet<Repository> Repositories =>
    Set<Repository>();

    public DbSet<RepositoryAnalysis> RepositoryAnalyses =>
        Set<RepositoryAnalysis>();

    public DbSet<RepositoryFile> RepositoryFiles =>
    Set<RepositoryFile>();

    public DbSet<CodeClass> CodeClasses =>
        Set<CodeClass>();

    public DbSet<CodeMethod> CodeMethods =>
        Set<CodeMethod>();

    public DbSet<CodeMethodParameter> CodeMethodParameters =>
        Set<CodeMethodParameter>();

    public DbSet<CodeDependency> CodeDependencies =>
        Set<CodeDependency>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    public DbSet<FileKnowledge> FileKnowledge =>
    Set<FileKnowledge>();

    public DbSet<CodeChunk> CodeChunks => Set<CodeChunk>();

    public DbSet<Embedding> Embeddings => Set<Embedding>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        // define the database schema, constraints, performance optimizations, and relationship behaviors
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(document => document.Id);

            entity.Property(document => document.Name)
                .IsRequired();

            entity.Property(document => document.RelativePath)
                .IsRequired();

            entity.Property(document => document.DocumentType)
                .IsRequired();

            entity.HasIndex(document => new
            {
                document.RepositoryId,
                document.RelativePath
            })
                .IsUnique();
        });

        modelBuilder.Entity<FileKnowledge>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Purpose)
                .HasMaxLength(2000);

            entity.Property(x => x.Summary)
                .HasMaxLength(5000);

            entity.Property(x => x.ArchitecturalRole)
                .HasMaxLength(200);

            entity.HasIndex(x => x.RepositoryFileId)
                .IsUnique();

            entity.HasOne<RepositoryFile>()
                .WithMany()
                .HasForeignKey(x => x.RepositoryFileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CodeChunk>(entity =>
        {
            entity.HasKey(chunk => chunk.Id);

            entity.Property(chunk => chunk.SymbolName)
                .HasMaxLength(500);

            entity.Property(chunk => chunk.SymbolType)
                .HasMaxLength(100);

            entity.Property(chunk => chunk.ContentHash)
                .HasMaxLength(128);

            entity.HasIndex(chunk => new
            {
                chunk.RepositoryFileId,
                chunk.ChunkIndex
            })
            .IsUnique();

            entity.HasIndex(chunk => chunk.CodeClassId);

            entity.HasIndex(chunk => chunk.CodeMethodId);

            entity.HasOne<RepositoryFile>()
                .WithMany()
                .HasForeignKey(chunk => chunk.RepositoryFileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Embedding>(entity =>
        {
            entity.HasKey(x => x.Id);

            // Convert domain float[] to Pgvector.Vector for storage in the 'vector' column
            entity.Property(x => x.Vector)
                .HasConversion(
                    v => new Vector(v),
                    v => v.ToArray())
                .HasColumnType("vector");

            entity.Property(x => x.Model)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Dimensions)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            //PostgreSQL unique indexes allow multiple NULLs, this works with our two nullable FKs.
            entity.HasIndex(x => x.DocumentChunkId)
                .IsUnique();

            entity.HasIndex(x => x.CodeChunkId)
                .IsUnique();

            entity.HasOne<DocumentChunk>()
                .WithMany()
                .HasForeignKey(x => x.DocumentChunkId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CodeChunk>()
                .WithMany()
                .HasForeignKey(x => x.CodeChunkId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Embeddings_ExactlyOneSource",
                    """
                    ("DocumentChunkId" IS NOT NULL AND "CodeChunkId" IS NULL)
                    OR
                    ("DocumentChunkId" IS NULL AND "CodeChunkId" IS NOT NULL)
                    """);
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}