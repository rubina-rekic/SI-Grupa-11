using Microsoft.Extensions.DependencyInjection;
using PostRoute.BLL.DependencyInjection;

namespace PostRoute.Api.Configuration;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddBusinessLayer(configuration);

        return services;
    }
}