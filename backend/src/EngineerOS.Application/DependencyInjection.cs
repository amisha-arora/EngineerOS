using Microsoft.Extensions.DependencyInjection;

namespace EngineerOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Application services will be registered here later.

        return services;
    }
}