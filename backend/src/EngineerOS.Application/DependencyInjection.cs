using EngineerOS.Application.Features.Authentication.Login;
using EngineerOS.Application.Features.Authentication.Register;
using EngineerOS.Application.Features.Authentication.Refresh;
using EngineerOS.Application.Features.Authentication.Logout;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using EngineerOS.Application.Features.Users.GetCurrentUser;
using EngineerOS.Application.Features.Repositories.Create;
using EngineerOS.Application.Features.Repositories.Delete;
using EngineerOS.Application.Features.Repositories.GetAll;
using EngineerOS.Application.Features.Repositories.GetById;

namespace EngineerOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<RegisterUserService>();
        services.AddScoped<LoginUserService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<LogoutService>();
        services.AddScoped<GetCurrentUserService>();
        services.AddScoped<CreateRepositoryService>();
        services.AddScoped<GetRepositoriesService>();
        services.AddScoped<GetRepositoryByIdService>();
        services.AddScoped<DeleteRepositoryService>();

        return services;
    }
}