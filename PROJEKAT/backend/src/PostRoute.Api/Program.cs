using PostRoute.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLayer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
