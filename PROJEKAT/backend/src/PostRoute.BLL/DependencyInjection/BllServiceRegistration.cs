using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostRoute.BLL.Services;
using PostRoute.DAL.DependencyInjection;

namespace PostRoute.BLL.DependencyInjection;

public static class BllServiceRegistration
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccessLayer(configuration);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSeedService, UserSeedService>();
        services.AddScoped<IBoxService, BoxService>();

        return services;
    }
}