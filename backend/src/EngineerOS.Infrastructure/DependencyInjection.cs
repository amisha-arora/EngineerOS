using EngineerOS.Application.Abstractions.Authentication;
using EngineerOS.Application.Abstractions.Persistence;
using EngineerOS.Infrastructure.Authentication;
using EngineerOS.Infrastructure.Persistence;
using EngineerOS.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using EngineerOS.Application.Abstractions.RepositoryFiles;
using EngineerOS.Infrastructure.FileStorage;
using EngineerOS.Application.Abstractions.Knowledge;
using EngineerOS.Application.Features.Repositories.Knowledge;
using EngineerOS.Infrastructure.Knowledge;
using Pgvector.EntityFrameworkCore;
using EngineerOS.Infrastructure.Knowledge.Voyage;
namespace EngineerOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Database connection string was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions.UseVector()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRepositoryRepository, RepositoryRepository>();
        services.AddScoped<IRepositoryFileStorage>(_ => new LocalRepositoryStorage(
            Path.Combine(Directory.GetCurrentDirectory(), "storage")));

        services.AddScoped<IRepositoryAnalysisRepository, RepositoryAnalysisRepository>();
        services.AddScoped<IRepositoryAnalysisWriter, RepositoryAnalysisWriter>();
        services.AddScoped<IRepositoryAnalysisReader, RepositoryAnalysisReader>();
        services.AddScoped<IDocumentationDetector, DocumentationDetector>();
        services.AddScoped<IDocumentationDiscoveryService,DocumentationDiscoveryService>();
        services.AddScoped<IContentExtractor, PlainTextContentExtractor>();
        services.AddScoped<IContentExtractor, PdfContentExtractor>();
        services.AddScoped<IContentExtractor, DocxContentExtractor>();
        services.AddScoped<IContentExtractorResolver, ContentExtractorResolver>();
        services.AddScoped<
        IRepositoryContentExtractionService>(
        provider =>
            new RepositoryContentExtractionService(
                provider.GetRequiredService<IRepositoryFileReader>(),
                provider.GetRequiredService<IContentExtractorResolver>(),
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "storage")));
        services.AddScoped<
            IFileImportanceAnalyzer,
            FileImportanceAnalyzer>();

        services.AddScoped<
            IRepositoryKnowledgeService,
            RepositoryKnowledgeService>();
        services.AddScoped<
            IDocumentChunker,
            DocumentChunker>();

        services.AddScoped<
            IDocumentChunkingService,
            DocumentChunkingService>();

        services.AddScoped<
            IFileKnowledgePreparationService,
            FileKnowledgePreparationService>();

        services.AddScoped<
            ICodeChunkPreparationService>(
            provider =>
                new CodeChunkPreparationService(
                    provider.GetRequiredService<ApplicationDbContext>(),
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "storage")));
        // Voyage configuration
        services.Configure<VoyageOptions>(
            configuration.GetSection(
                VoyageOptions.SectionName));

        // Voyage embedding provider
        services.AddHttpClient<IEmbeddingProvider, VoyageEmbeddingProvider>(client =>
        {
            client.BaseAddress = new Uri("https://api.voyageai.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<
            IRepositoryKnowledgeIndexingService,
            RepositoryKnowledgeIndexingService>();
        return services;
    }
}
