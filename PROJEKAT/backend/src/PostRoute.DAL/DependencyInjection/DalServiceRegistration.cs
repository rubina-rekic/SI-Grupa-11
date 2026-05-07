using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostRoute.DAL.Repositories;

namespace PostRoute.DAL.DependencyInjection;

public static class DalServiceRegistration
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISecurityLogRepository, SecurityLogRepository>();
        services.AddScoped<IMailboxRepository, MailboxRepository>();
        services.AddScoped<IMailboxAuditLogRepository, MailboxAuditLogRepository>();

        return services;
    }
}