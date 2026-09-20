using CourseHub.Application.Interfaces.Services;
using CourseHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}