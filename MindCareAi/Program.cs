using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Infrastructure.Extensions;
using MindCareAi.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version"),
            new QueryStringApiVersionReader("api-version"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/openapi/{description.GroupName}.json",
                $"MindCare AI {description.GroupName.ToUpperInvariant()}");
        }
        options.RoutePrefix = "swagger-ui";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

await EnsureDatabaseAsync(app.Services);

app.Run();

async Task EnsureDatabaseAsync(IServiceProvider services)
{
    await using var scope = services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<MindCareContextInitializer>();
    await initializer.InitializeAsync();
}
