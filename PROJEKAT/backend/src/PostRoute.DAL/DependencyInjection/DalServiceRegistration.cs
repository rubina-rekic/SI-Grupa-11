using Microsoft.Extensions.DependencyInjection;
using PostRoute.DAL.Repositories;

namespace PostRoute.DAL.DependencyInjection;

public static class DalServiceRegistration
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepositoryPlaceholder>();

        return services;
    }
}
