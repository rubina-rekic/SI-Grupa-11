using Microsoft.Extensions.DependencyInjection;
using PostRoute.BLL.DependencyInjection;

namespace PostRoute.Api.Configuration;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddBusinessLayer();

        return services;
    }
}
