using Microsoft.AspNetCore.Http;
using System.Text.Json;
using PostRoute.Domain.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.DAL.Entities;

namespace PostRoute.Api.Middleware;

public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public RoleAuthorizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var requiredRoleAttribute = endpoint.Metadata.GetMetadata<RequiredRoleAttribute>();
            if (requiredRoleAttribute != null)
            {
                var userRole = context.Session.GetString("UserRole");
                var userId = context.Session.GetString("UserId");
                
                if (string.IsNullOrEmpty(userRole) || userRole != requiredRoleAttribute.Role)
                {
                    // Log unauthorized access attempt
                    await LogSecurityEvent(context, userId, userRole, "Forbidden");
                    
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var response = new { message = "Pristup odbijen: Nemate potrebne privilegije za ovu akciju." };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
            }
        }

        await _next(context);
    }

    private async Task LogSecurityEvent(HttpContext context, string? userId, string? userRole, string accessType)
    {
        using var scope = _serviceProvider.CreateScope();
        var securityLogRepository = scope.ServiceProvider.GetRequiredService<ISecurityLogRepository>();
        
        var securityLog = new SecurityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AttemptedUrl = context.Request.Path + context.Request.QueryString,
            UserRole = userRole,
            IpAddress = GetClientIpAddress(context),
            AccessType = accessType,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            IsSuccessful = false
        };

        await securityLogRepository.AddAsync(securityLog, CancellationToken.None);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress.Split(',')[0].Trim();
        }

        ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

public class RequiredRoleAttribute : Attribute
{
    public string Role { get; }

    public RequiredRoleAttribute(string role)
    {
        Role = role;
    }
}
