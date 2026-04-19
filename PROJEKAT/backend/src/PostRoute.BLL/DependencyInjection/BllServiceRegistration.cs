using Microsoft.Extensions.DependencyInjection;
using PostRoute.BLL.Services;
using PostRoute.DAL.DependencyInjection;

namespace PostRoute.BLL.DependencyInjection;

public static class BllServiceRegistration
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddDataAccessLayer();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
