using MindCareAi.Infrastructure.Data;
using MindCareAi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await EnsureDatabaseAsync(app.Services);

app.Run();

async Task EnsureDatabaseAsync(IServiceProvider services)
{
    await using var scope = services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<MindCareContextInitializer>();
    await initializer.InitializeAsync();
}
