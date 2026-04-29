using PostRoute.Api.Configuration;
using PostRoute.Api.Middleware;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5174")
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

app.Run();