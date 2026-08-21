using EngineerOS.Application.Features.Authentication.Login;
using EngineerOS.Application.Features.Authentication.Register;
using EngineerOS.Application.Features.Authentication.Refresh;
using EngineerOS.Application.Features.Authentication.Logout;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using EngineerOS.Application.Features.Users.GetCurrentUser;

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

        return services;
    }
}