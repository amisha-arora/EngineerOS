using Microsoft.EntityFrameworkCore;
using EngineerOS.Domain.Entities;

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

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}