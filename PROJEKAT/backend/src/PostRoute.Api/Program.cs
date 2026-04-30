using Microsoft.EntityFrameworkCore;
using PostRoute.Api.Configuration;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Services;
using PostRoute.DAL;
using PostRoute.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLayer(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:5174"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");
app.UseSession();
app.UseMiddleware<RoleAuthorizationMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Lightweight health endpoint used by Docker Compose healthcheck and Nginx proxy.
app.MapGet("/health", () => Results.Ok());

// Apply pending migrations on startup so a fresh deployment ends up with the
// right schema before traffic arrives. Seeding is gated on Seeding:Enabled so
// it can be turned off in environments where default users are not desired.
// Default: enabled in Development, disabled elsewhere unless explicitly turned
// on via env var (Seeding__Enabled=true) or appsettings.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync(CancellationToken.None);

    var seedingEnabled = app.Configuration.GetValue<bool?>("Seeding:Enabled")
        ?? app.Environment.IsDevelopment();

    if (seedingEnabled)
    {
        var userSeedService = scope.ServiceProvider.GetRequiredService<IUserSeedService>();
        await userSeedService.SeedDefaultUsersAsync(CancellationToken.None);
    }
}

app.Run();
