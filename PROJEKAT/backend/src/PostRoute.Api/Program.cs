using Microsoft.EntityFrameworkCore;
using PostRoute.Api.Configuration;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Services;
using PostRoute.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLayer(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // SameSite=None + Secure potrebno kada su frontend i backend na različitim
    // domenama (npr. netlify.app i onrender.com). Bez ovoga browser blokira
    // slanje session cookie-a u cross-site zahtjevima.
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

// Health endpoint za Render (i bilo koji uptime monitor).
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Auto-migracija na startup-u + opcionalno seed defaultnih korisnika.
// Seeding se kontroliše s Seeding:Enabled iz konfiguracije; uključi ga
// na prvi deploy da admin nalog postoji, pa ga ugasi sljedećim deploy-em.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var seedingEnabled = app.Configuration.GetValue<bool?>("Seeding:Enabled")
        ?? app.Environment.IsDevelopment();

    if (seedingEnabled)
    {
        var seedService = scope.ServiceProvider.GetRequiredService<IUserSeedService>();
        await seedService.SeedDefaultUsersAsync(CancellationToken.None);
    }
}

app.Run();
